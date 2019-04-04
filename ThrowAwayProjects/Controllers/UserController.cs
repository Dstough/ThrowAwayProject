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

        public ActionResult Index(int id)
        {
            return HandleExceptions(() =>
            {
                var dbUsers = unitOfWork.Users.GetPage(id, 50);
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

        public JsonResult AddEdit()
        {
            return HandleExceptions(() =>
            {
                return Modal("AddEdit", new UserViewModel());
            });
        }

        protected override ActionResult HandleExceptions(Func<ActionResult> logic)
        {
            try
            {
                if (!IsAuthenticated())
                    return RedirectToAction("Index", "Home");

                var user = JsonConvert.DeserializeObject<UserIdentity>(configuration.GetValue<string>("UserKey"));

                if (unitOfWork.UserGroups.GetById(user.GroupId).Name != "Admin")
                    return RedirectToAction("Index", "Home");
                
                return logic();
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }
    }
}