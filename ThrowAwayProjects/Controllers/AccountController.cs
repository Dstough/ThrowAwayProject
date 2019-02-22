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
using ThrowAwayData;
using Newtonsoft.Json;

namespace ThrowAwayProjects.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(ICompositeViewEngine viewEngine, IConfiguration configuration, IHostingEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult LogIn()
        {
            try
            {
                var model = new UserViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult Register()
        {
            try
            {
                var model = new UserIdentity();
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        [HttpPost]
        public ActionResult LogIn(UserViewModel viewModel)
        {
            try
            {
                var dbUser = unitOfWork.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "UserName",
                        Value = "'" + viewModel.UserName + "'"
                    }
                }).FirstOrDefault();

                if (Sha512(viewModel.PassPhrase + dbUser.CreatedOn) == dbUser.PassPhrase)
                {
                    HttpContext.Session.SetString(configuration.GetValue<string>("UserKey"), JsonConvert.SerializeObject(dbUser));
                    return RedirectToAction(viewModel.TargetAction, viewModel.TargetController);
                }
                else
                    return View("LogIn", viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        [HttpPost]
        public ActionResult Register(UserViewModel viewModel)
        {
            try
            {
                var CreatedDate = DateTime.Now;
                var user = new UserIdentity
                {
                    UserName = viewModel.UserName,
                    CreatedOn = CreatedDate,
                    Email = viewModel.Email,
                    PassPhrase = Sha512(viewModel.PassPhrase + CreatedDate)
                };

                unitOfWork.Users.Add(user);

                //TODO: maybe auto log in when they register.
                return RedirectToAction("LogIn");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }
    }
}