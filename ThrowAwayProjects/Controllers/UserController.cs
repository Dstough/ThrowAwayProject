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
    }
}