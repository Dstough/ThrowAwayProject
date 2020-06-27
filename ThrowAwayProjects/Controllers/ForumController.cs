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

                var dbAuthor = database.Users.Get(dbThread.CreatedBy);
                var dbGroup = database.UserGroups.Get(dbAuthor.UserGroupId);

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
                    Id = dbThread.PublicId,
                    Title = dbThread.Title,
                    Body = dbThread.Body,
                    Author = dbAuthor.UserName,
                    PostDate = dbAuthor.CreatedOn,
                    Edited = dbThread.Edited,
                    Style = dbGroup.Style + " " + dbAuthor.Style,
                    Posts = new List<PostViewModel>()
                };

                var dbPosts = database.Posts.Where(new { ThreadId = dbThread.Id.ToString() }).Find();

                dbPosts.ToList().ForEach(dbPost =>
                {
                    var author = database.Users.Get(dbPost.CreatedBy);
                    var group = database.UserGroups.Get(author.UserGroupId);

                    if (author == null)
                        throw new Exception("A post did not have a valid author.");

                    var post = new PostViewModel
                    {
                        Id = dbPost.PublicId,
                        Body = dbPost.Body,
                        Author = author.UserName,
                        PostDate = dbPost.CreatedOn,
                        Edited = dbPost.Edited,
                        Style = group.Style + " " + author.Style
                    };

                    viewModel.Posts.Add(post);

                });

                return View(viewModel);
            });
        }

        public ActionResult List(int Id = 1)
        {
            return HandleExceptions(() =>
            {
                var dbThreads = database.Threads.GetPage(Id, 20);
                var viewModel = new ListViewModel(Id, 20);

                foreach (var dbThread in dbThreads)
                {
                    var user = database.Users.Get(dbThread.CreatedBy);
                    var group = database.UserGroups.Get(user.UserGroupId);

                    viewModel.threads.Add(new ThreadViewModel(dbThread)
                    {
                        Style = group.Style + " " + user.Style,
                        Author = user.UserName,
                        Body = dbThread.Body.Length > 255 ? dbThread.Body.Substring(0, 255) + "..." : dbThread.Body
                    });
                }

                return View(viewModel);
            });
        }

        public JsonResult AddEditThread(string Id)
        {
            return HandleExceptions(() =>
            {
                if (Id == null)
                    return Modal("_AddEditThread", new ThreadViewModel());

                HttpContext.Session.SetString("CurrentEditId", Id);

                var dbThread = database.Threads.Where(new { PublicId = Id }).Find().FirstOrDefault();

                if (dbThread == null)
                    throw new Exception("That thread couldn't be found in the database.");

                var currentUserId = Convert.ToInt32(HttpContext.Session.GetString("CurrentUserId"));

                if (currentUserId != dbThread.CreatedBy)
                    throw new Exception("You cannot edit a thread that you didn't write.");

                var viewModel = new ThreadViewModel(dbThread);

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
                    dbThread.Title = viewModel.Title;
                    dbThread.Body = viewModel.Body;
                    dbThread.CreatedBy = user.Id ?? 0;
                    database.Threads.Add(dbThread);
                }
                else
                {
                    dbThread = database.Threads.Where(new { PublicId = currentEditId }).Find().FirstOrDefault();
                    dbThread.Title = viewModel.Title;
                    dbThread.Body = viewModel.Body;
                    dbThread.Edited = true;
                    database.Threads.Edit(dbThread);
                }

                return RedirectToAction("Thread", new { Id = dbThread.PublicId });
            });
        }

        public JsonResult AddPost(string Id)
        {
            return HandleExceptions(() =>
            {
                var userKey = HttpContext.Session.GetString("UserKey");

                if (userKey == null)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var sessionUser = JsonConvert.DeserializeObject<UserIdentity>(userKey);

                if (Id == null)
                    throw new Exception("You need to specify a post to edit chummer.");

                var dbThread = database.Threads.Where(new { PublicId = Id }).Find().FirstOrDefault();

                if (dbThread == null)
                    throw new Exception("That isn't a valid post omae.");

                HttpContext.Session.SetString("CurrentThreadId", Id);
                HttpContext.Session.SetString("CurrentPostId", "");

                var viewModel = new PostViewModel();

                return Modal("_AddEditPost", viewModel);
            });
        }

        public JsonResult EditPost(string Id)
        {
            return HandleExceptions(() => 
            {
                var userKey = HttpContext.Session.GetString("UserKey");

                if (userKey == null)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var sessionUser = JsonConvert.DeserializeObject<UserIdentity>(userKey);

                if (Id == null)
                    throw new Exception("You need to specify a post to edit chummer.");

                HttpContext.Session.SetString("CurrentPostId", Id);

                var dbPost = database.Posts.Where(new { PublicId = Id }).Find().FirstOrDefault();

                if (dbPost == null)
                    throw new Exception("That isn't a valid post omae.");

                var dbAuthor = database.Users.Get(dbPost.CreatedBy);

                if (dbAuthor == null)
                    throw new Exception("The post has no author.");

                var dbGroup = database.UserGroups.Get(dbAuthor.UserGroupId);

                if (dbGroup == null)
                    throw new Exception("The user is not valid.");

                var viewModel = new PostViewModel()
                {
                    Author = dbAuthor.UserName,
                    Body = dbPost.Body,
                    Edited = dbPost.Edited,
                    Id = dbPost.PublicId,
                    PostDate = dbPost.CreatedOn,
                    Style = dbGroup.Style + dbAuthor.Style
                };

                return Modal("_AddEditPost", viewModel);
            });
        }

        [HttpPost]
        public ActionResult AddEditPost(PostViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var userKey = HttpContext.Session.GetString("UserKey");
                var currentThreadId = HttpContext.Session.GetString("CurrentThreadId");
                var currentPostId = HttpContext.Session.GetString("CurrentPostId");

                if (userKey == null || currentThreadId == null)
                    throw new Exception("Stop poking around where you shouldn't be omae!");

                var sessionUser = JsonConvert.DeserializeObject<UserIdentity>(userKey);
                var dbThread = database.Threads.Where(new { PublicId = currentThreadId }).Find().FirstOrDefault();

                if (dbThread == null)
                    throw new Exception("That thread doesn't exist. Sorry chummer.");

                if (string.IsNullOrEmpty(currentPostId))
                {
                    var dbPost = new Post()
                    {
                        Body = viewModel.Body,
                        CreatedBy = sessionUser.Id ?? 0,
                        CreatedOn = DateTime.Now,
                        Deleted = false,
                        Edited = false,
                        ThreadId = dbThread.Id ?? 0
                    };
                    database.Posts.Add(dbPost);
                }
                else
                {
                    var dbPost = database.Posts.Where(new { PublicId = currentPostId }).Find().FirstOrDefault();

                    if (dbPost == null)
                        throw new Exception("That post isn't able to be edited.");

                    dbPost.Body = viewModel.Body;
                    dbPost.Edited = true;
                    database.Posts.Edit(dbPost);
                }

                return RedirectToAction("Thread", new { Id = dbThread.PublicId });
            });
        }
    }
}
