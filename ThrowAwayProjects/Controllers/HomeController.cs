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

                foreach (var thread in database.Threads.Find(5))
                {
                    var author = database.Users.Include("UserGroup").Get(thread.CreatedBy);
                    viewModel.UserThreads.Add(new ThreadViewModel(thread)
                    {
                        Author = author.UserName,
                        CSS = author.UserGroup.Name == "Admin" ? "admin-color" : ""
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
                var dbUser = database.Users.Where(new { viewModel.UserName }).Include("UserGroup").Find().FirstOrDefault();

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
                var thread = database.Threads.GetRandom();
                var author = database.Users.Include("UserGroup").Get(thread.CreatedBy);

                return Json(new
                {
                    message = thread.Title,
                    signature = author.UserName,
                    css = author.UserGroup.Name == "Admin" ? "admin-color" : "",
                    url = "/Forum/Thread/" + thread.PublicId
                });
            });
        }
    }
}
