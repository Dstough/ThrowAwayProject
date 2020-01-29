﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using Newtonsoft.Json;

namespace ThrowAwayProjects.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ICompositeViewEngine viewEngine, IConfiguration configuration, IWebHostEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var viewModel = new HomeViewModel();

                foreach (var thread in database.Threads.Find(3))
                {
                    var author = database.Users.Get(thread.CreatedBy);
                    var group = database.UserGroups.Get(author.UserGroupId);

                    viewModel.UserThreads.Add(new ThreadViewModel(thread)
                    {
                        Body = thread.Body.Length > 255 ? thread.Body.Substring(0,255) + "..." : thread.Body,
                        Author = author.UserName,
                        Style = group.Style + " " + author.Style,
                        PostDate = thread.CreatedOn
                    });
                }

                return View(viewModel);
            });
        }

        [HttpPost]
        public ActionResult Index(AccountViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = database.Users.Where(new { viewModel.UserName }).Find().FirstOrDefault();

                if (dbUser == null || Sha512(viewModel.Passphrase + dbUser.CreatedOn) != dbUser.Passphrase)
                    throw new Exception("I couldn't find your account information. You eather had the wrong username or passphrase.");

                var dbGroup = database.UserGroups.Get(dbUser.UserGroupId);

                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(dbUser));
                HttpContext.Session.SetString("GroupKey", JsonConvert.SerializeObject(dbGroup));

                if (!dbUser.Authenticated)
                {
                    //TODO: Send Auth Code to email.
                    return RedirectToAction("Verify", "Account");
                }

                return RedirectToAction("Index", "Home");
            });
        }

        public JsonResult GetRandomPost()
        {
            return HandleExceptions(() =>
            {
                var thread = database.Threads.GetRandom();
                var author = database.Users.Get(thread.CreatedBy);
                var group = database.UserGroups.Get(author.UserGroupId);

                return Json(new
                {
                    message = thread.Title.Substring(0, 200) + "...",
                    signature = author.UserName,
                    style = group.Style + " " + author.Style,
                    url = "/Forum/Thread/" + thread.PublicId,
                    date = thread.CreatedOn.AddYears(60).ToString("MM/dd/yyyy h:mm:ss tt")
                });
            });
        }
    }
}
