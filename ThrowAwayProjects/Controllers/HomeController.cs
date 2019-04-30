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
        public HomeController(ICompositeViewEngine viewEngine, IConfiguration configuration, IWebHostEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var viewModel = new HomeViewModel();
                return View(viewModel);
            });
        }

        [HttpPost]
        public ActionResult Index(AccountViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = database.Users.Where(new { UserName = viewModel.UserName }).Include("UserGroup").Find().FirstOrDefault();

                if (dbUser == null || Sha512(viewModel.Passphrase + dbUser.CreatedOn) != dbUser.Passphrase)
                {
                    var model = new HomeViewModel()
                    {
                        ErrorMessage = "I couldn't find your account information. You eather had the wrong username or Passphrase."
                    };
                    return View("../Home/Index", model);
                }

                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(dbUser));

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
                //TODO: get a random post title from the db here.

                var list = new List<string> { "Hello world", "how are you?", "what is up?" };
                var message = list.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                return Json(new
                {
                    message = message,
                    signature = "-- Fastjack",
                    css = "admin-color"
                });
            });
        }
    }
}
