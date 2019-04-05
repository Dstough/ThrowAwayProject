using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using ThrowAwayData;
using Newtonsoft.Json;

namespace ThrowAwayProjects.Controllers
{
    public class UserController : BaseController
    {
        public UserController(ICompositeViewEngine viewEngine, IConfiguration configuration, IHostingEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        protected override ActionResult HandleExceptions(Func<ActionResult> logic)
        {
            try
            {
                if (!IsAuthenticated())
                    return RedirectToAction("Index", "Home");

                var key = HttpContext.Session.GetString("UserKey");
                var user = JsonConvert.DeserializeObject<UserIdentity>(key);

                if (unitOfWork.UserGroups.GetById(user.GroupId).Name != "Admin")
                    return RedirectToAction("Index", "Home");

                return logic();
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult Index(int id)
        {
            return HandleExceptions(() =>
            {
                var dbUsers = unitOfWork.Users.GetPage(id+1, 50);
                var viewModel = new List<UserViewModel>();

                foreach (var user in dbUsers)
                {
                    viewModel.Add(new UserViewModel(user)
                    {
                        GroupName = unitOfWork.UserGroups.GetById(user.GroupId).Name
                    });
                }

                return View(viewModel);
            });
        }

        public JsonResult AddEdit(int Id)
        {
            return HandleExceptions(() =>
            {
                return Modal("_AddEdit", new UserViewModel());
            });
        }

        [HttpPost]
        public JsonResult AddEdit(UserViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                //TODO: add html string to update row.
                return Json(new
                {
                    message = "The user info has been saved.",
                    html = ""
                });
            });
        }
    }
}