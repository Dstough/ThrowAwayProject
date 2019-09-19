using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using Microsoft.AspNetCore.Http;
using ThrowAwayData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ThrowAwayProjects.Controllers
{
    public class ForumController : BaseController
    {
        public ForumController(ICompositeViewEngine viewEngine, IConfiguration configuration, IWebHostEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult Thread(int Id)
        {
            return HandleExceptions(() =>
            {
                if (Id == 0)
                    throw new Exception("The post number needs to be specified in the address bar.");

                var dbThread = database.Threads.Get(Id);

                if (dbThread == null)
                    throw new Exception("The post doesn't exist.");

                var dbAuthor = database.Users.Include("UserGroup").Get(dbThread.CreatedBy);

                if (dbAuthor == null)
                    throw new Exception("The post doesn't have a valid user.");

                if (HttpContext.Session.GetString("UserKey") != null)
                {
                    var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                    HttpContext.Session.SetString("CurrentUserId", user.Id.ToString());
                    HttpContext.Session.SetString("CurrentThreadId", Id.ToString());
                }

                var viewModel = new ThreadViewModel
                {
                    Title = dbThread.Title,
                    Body = dbThread.Body,
                    Author = dbAuthor.UserName,
                    PostDate = dbAuthor.CreatedOn,
                    CSS = dbAuthor.UserGroup.Name == "Admin" ? "admin-color" : "",
                    Posts = new List<PostViewModel>()
                };

                var dbPosts = database.Posts.Where(new { ThreadId = dbThread.Id.ToString() }).Find();

                dbPosts.ToList().ForEach(dbPost =>
                {
                    var author = database.Users.Include("UserGroup").Get(dbPost.CreatedBy);

                    if (author == null)
                        throw new Exception("A post did not have a valid author.");

                    var post = new PostViewModel
                    {
                        Id = dbPost.Id,
                        Body = dbPost.Body,
                        Author = author.UserName,
                        PostDate = dbPost.CreatedOn,
                        CSS = author.UserGroup.Name == "Admin" ? "admin-color" : ""
                    };

                    viewModel.Posts.Add(post);

                });

                return View(viewModel);
            });
        }

        public ActionResult List(int Page = 0)
        {
            return HandleExceptions(() => 
            {
                var dbThreads = database.Threads.GetPage(Page, 20);
                var threads = new List<ThreadViewModel>();

                foreach (var dbThread in dbThreads)
                {
                    var user = database.Users.Include("UserGroups").Get(dbThread.CreatedBy);

                    threads.Add(new ThreadViewModel(dbThread)
                    {
                        CSS = user.UserGroup.Name == "Admin" ? "admin-color" : "",
                        Author = user.UserName
                    });
                }
                
                return View(threads);
            });
        }

        [HttpPost]
        public ActionResult AddPost(PostViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var User = HttpContext.Session.GetString("CurrentUserId");
                var ThreadId = HttpContext.Session.GetString("CurrentThreadId");

                if (User == null || ThreadId == null)
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var dbPost = new Post()
                {
                    Body = viewModel.Body,
                    CreatedBy = Convert.ToInt32(User),
                    ThreadId = Convert.ToInt32(ThreadId)
                };

                database.Posts.Add(dbPost);

                return RedirectToAction("Thread", new { Id = ThreadId });
            });
        }

        public JsonResult AddEditThread(int Id)
        {
            return HandleExceptions(() =>
            {
                HttpContext.Session.SetString("CurrentEditId", Id.ToString());

                if (Id == 0)
                    return Modal("_AddEditThread", new ThreadViewModel());

                var DBthread = database.Threads.Get(Id);

                if (DBthread == null)
                    throw new Exception("That thread couldn't be found in the database.");

                var viewModel = new ThreadViewModel()
                {
                    Title = DBthread.Title,
                    Body = DBthread.Body
                };

                return Modal("_AddEditThread", viewModel);
            });
        }

        [HttpPost]
        public ActionResult AddEditThread(ThreadViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var key = HttpContext.Session.GetString("UserKey");

                if (key == null)
                    throw new Exception("You must be logged in to do that.");

                var user = JsonConvert.DeserializeObject<UserIdentity>(key);

                var sessionString = HttpContext.Session.GetString("CurrentEditId");

                if (sessionString == null)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var dbThread = new Thread();

                if (sessionString == "0")
                {
                    dbThread.Title = viewModel.Title;
                    dbThread.Body = viewModel.Body;
                    dbThread.CreatedBy = user.Id ?? 0;
                    database.Threads.Add(dbThread);
                }
                else
                {
                    dbThread = database.Threads.Get(Convert.ToInt32(sessionString));
                    dbThread.Title = viewModel.Title;
                    dbThread.Body = viewModel.Body;
                    database.Threads.Edit(dbThread);
                }

                return RedirectToAction("Thread", new { Id = dbThread.Id ?? 0 });
            });
        }

        public JsonResult EditPost(int? Id)
        {
            return HandleExceptions(() =>
            {
                var userKey = HttpContext.Session.GetString("UserKey");

                if (userKey == null)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var sessionUser = JsonConvert.DeserializeObject<UserIdentity>(userKey);

                if (Id == null)
                    throw new Exception("You need to specify a post to edit chummer.");

                var dbPost = database.Posts.Get(Id ?? 0);

                if(dbPost == null)
                    throw new Exception("That isn't a valid post omae.");

                if (dbPost.CreatedBy != sessionUser.Id)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var viewModel = new PostViewModel() {
                    Id = dbPost.Id,
                    Author = sessionUser.UserName,
                    Body = dbPost.Body,
                    PostDate = dbPost.CreatedOn
                };

                return Modal("_EditPost",viewModel);
            });
        }

        [HttpPost]
        public JsonResult EditPost(PostViewModel viewModel)
        {
            return HandleExceptions(() =>
            {

                return Json(new { });
            });
        }
    }
}
