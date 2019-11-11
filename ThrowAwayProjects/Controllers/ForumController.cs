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

        public ActionResult Thread(string Id)
        {
            return HandleExceptions(() =>
            {
                if (Id == null)
                    throw new Exception("The thread id needs to be specified in the address bar.");

                var dbThread = database.Threads.Where(new { PublicId = Id }).Find().FirstOrDefault();

                if (dbThread == null)
                    throw new Exception("The thread doesn't exist.");

                var dbAuthor = database.Users.Include("UserGroup").Get(dbThread.CreatedBy);

                if (dbAuthor == null)
                    throw new Exception("The thread doesn't have a valid user.");

                if (HttpContext.Session.GetString("UserKey") != null)
                {
                    var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                    HttpContext.Session.SetString("CurrentUserId", user.Id.ToString());
                    HttpContext.Session.SetString("CurrentThreadId", Id.ToString());
                }

                var viewModel = new ThreadViewModel
                {
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
                    var user = database.Users.Get(dbThread.CreatedBy);
                    var group = database.UserGroups.Get(user.UserGroupId);

                    threads.Add(new ThreadViewModel(dbThread)
                    {
                        CSS = group.Name == "Admin" ? "admin-color" : "",
                        Author = user.UserName
                    }); ;
                }

                return View(threads);
            });
        }

        public JsonResult AddEditThread(string id)
        {
            return HandleExceptions(() =>
            {
                if (id == null)
                    return Modal("_AddEditThread", new ThreadViewModel());

                HttpContext.Session.SetString("CurrentEditId", id);

                var dbThread = database.Threads.Where(new { PublicId = id }).Find().FirstOrDefault();

                if (dbThread == null)
                    throw new Exception("That thread couldn't be found in the database.");

                var viewModel = new ThreadViewModel()
                {
                    Body = dbThread.Body.Substring(0, 200) + "..."
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
                var currentEditId = HttpContext.Session.GetString("CurrentEditId");
                var dbThread = new Thread();

                if (key == null)
                    throw new Exception("You must be logged in to do that.");

                var user = JsonConvert.DeserializeObject<UserIdentity>(key);

                if (currentEditId == null)
                {
                    dbThread.Body = viewModel.Body;
                    dbThread.CreatedBy = user.Id ?? 0;
                    database.Threads.Add(dbThread);
                }
                else
                {
                    dbThread = database.Threads.Where(new { PublicId = currentEditId }).Find().FirstOrDefault();
                    dbThread.Body = viewModel.Body;
                    database.Threads.Edit(dbThread);
                }

                return RedirectToAction("Thread", new { Id = dbThread.PublicId });
            });
        }

        [HttpPost]
        public ActionResult AddPost(PostViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var user = HttpContext.Session.GetString("CurrentUserId");
                var threadId = HttpContext.Session.GetString("CurrentThreadId");

                if (user == null || threadId == null)
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var dbPost = new Post()
                {
                    Body = viewModel.Body,
                    CreatedBy = Convert.ToInt32(User),
                    ThreadId = Convert.ToInt32(threadId)
                };

                database.Posts.Add(dbPost);

                return RedirectToAction("Thread", new { Id = threadId });
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

                if (dbPost == null)
                    throw new Exception("That isn't a valid post omae.");

                if (dbPost.CreatedBy != sessionUser.Id)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var viewModel = new PostViewModel()
                {
                    Id = dbPost.Id,
                    Author = sessionUser.UserName,
                    Body = dbPost.Body,
                    PostDate = dbPost.CreatedOn
                };

                return Modal("_EditPost", viewModel);
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
