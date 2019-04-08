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
        protected override JsonResult HandleExceptions(Func<JsonResult> logic)
        {
            try
            {
                if (!IsAuthenticated())
                    return Json(new { result = "error", message = "Stop poking around where you shouldn't be omae." });

                var key = HttpContext.Session.GetString("UserKey");
                var user = JsonConvert.DeserializeObject<UserIdentity>(key);

                if (unitOfWork.UserGroups.GetById(user.GroupId).Name != "Admin")
                    return Json(new { result = "error", message = "Stop poking around where you shouldn't be omae." });

                return logic();
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }

        public ActionResult Index(int id)
        {
            return HandleExceptions(() =>
            {
                var dbUsers = unitOfWork.Users.GetPage(id + 1, 50);
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
                var dbUser = unitOfWork.Users.GetById(Id);
                var group = unitOfWork.UserGroups.GetById(dbUser.GroupId);
                var groups = unitOfWork.UserGroups.GetAll();
                var viewModel = new UserViewModel(dbUser)
                {
                    Passphrase = null
                };

                foreach (var item in groups)
                    viewModel.GroupOptions.Add(new SelectListItem(group.Name, group.Id.ToString(), group.Id == item.Id));

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