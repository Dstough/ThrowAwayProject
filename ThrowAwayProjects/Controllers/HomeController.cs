using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using Newtonsoft.Json;
using ThrowAwayData;

namespace ThrowAwayProjects.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ICompositeViewEngine viewEngine, IConfiguration configuration, IHostingEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var viewModel = new HomeViewModel();

                if (HttpContext.Session.GetString("FirstSeen") == null)
                    HttpContext.Session.SetString("FirstSeen", JsonConvert.SerializeObject(viewModel.DateSessionStarted));
                else
                    viewModel.DateSessionStarted = JsonConvert.DeserializeObject<DateTime>(HttpContext.Session.GetString("FirstSeen"));

                var dbGroups = database.UserGroups.Include("UserIdentity").GetById(3);

                return View(viewModel);
            });
        }

        [HttpPost]
        public ActionResult Index(AccountViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = database.Users.Include("UserGroup").Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "UserName",
                        Value = viewModel.UserName
                    }
                }).FirstOrDefault();

                if (dbUser == null || Sha512(viewModel.Passphrase + dbUser.CreatedOn) != dbUser.Passphrase)
                {
                    var model = new HomeViewModel()
                    {
                        ErrorMessage = "I couldn't find your account information. You eather had the wrong username or Passphrase."
                    };
                    return View("../Home/Index", model);
                }

                SetSessionUser(dbUser);

                if (!dbUser.Authenticated)
                {
                    //TODO: Send Auth Code to email.
                    return RedirectToAction("Verify", "Account");
                }

                return RedirectToAction("Index", "Home");
            });
        }

        private void SetSessionUser(UserIdentity user)
        {
            var dbUserGroup = database.UserGroups.GetById(user.UserGroupId).Name;
            HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(user));
            HttpContext.Session.SetString("UserGroup", dbUserGroup);
        }
    }
}
