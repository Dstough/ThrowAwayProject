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

        public JsonResult LogIn()
        {
            return HandleExceptions(() =>
            {
                var model = new AccountViewModel();
                return Modal("_LogIn", model);
            });
        }

        public ActionResult LogOut()
        {
            return HandleExceptions(() =>
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            });
        }

        public ActionResult Register()
        {
            return HandleExceptions(() =>
            {
                var model = new AccountViewModel();
                return View(model);
            });
        }

        [HttpPost]
        public ActionResult Register(AccountViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var CreatedDate = DateTime.Now;
                var defaultUserGroup = unitOfWork.UserGroups.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "Name",
                        Value = "User"
                    }
                }).FirstOrDefault();

                var userNameCheck = unitOfWork.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "UserName",
                        Value = viewModel.UserName
                    }
                }).FirstOrDefault();

                var emailCheck = unitOfWork.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "Email",
                        Value = viewModel.Email
                    }
                }).FirstOrDefault();

                if (defaultUserGroup == null)
                    viewModel.ErrorMessage = "We can't let you do that Omae.";

                if (emailCheck != null)
                    viewModel.ErrorMessage = "That email is already in use.";

                if (userNameCheck != null)
                    viewModel.ErrorMessage = "That username is already taken.";

                if (viewModel.PassPhrase != viewModel.PassPhraseConfirm)
                    viewModel.ErrorMessage = "Your passphrases didn't match.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                var user = new UserIdentity
                {
                    GroupId = defaultUserGroup.Id ?? 0,
                    UserName = viewModel.UserName,
                    CreatedOn = CreatedDate,
                    Email = viewModel.Email,
                    PassPhrase = Sha512(viewModel.PassPhrase + CreatedDate),
                    VerificationCode = Guid.NewGuid().ToString()
                };
                unitOfWork.Users.Add(user);
                SetSessionUser(user);

                //TODO: Send Auth Code to email.
                return RedirectToAction("Verify");
            });
        }

        public ActionResult Verify()
        {
            return HandleExceptions(() =>
            {
                if (HttpContext.Session.GetString("UserKey") == null)
                    return RedirectToAction("Index", "Home");

                var user = GetSessionUser();
                var model = new VerificationViewModel()
                {
                    UserId = user.Id,
                    dbGuid = user.VerificationCode
                };
                return View(model);
            });
        }

        [HttpPost]
        public ActionResult Verify(VerificationViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = unitOfWork.Users.GetById(viewModel.UserId ?? 0);
                var dbUserGroup = unitOfWork.UserGroups.GetById(dbUser.GroupId)?.Name;
                if (viewModel.inputGuid == dbUser.VerificationCode)
                {
                    dbUser.Authenticated = true;
                    unitOfWork.Users.Edit(dbUser);
                    SetSessionUser(dbUser);
                    return RedirectToAction("Index", "Home");
                }
                viewModel.ErrorMessage = "The verification code was not input correctly.";
                return View(viewModel);
            });
        }

        public ActionResult Recovery()
        {
            return HandleExceptions(() =>
            {
                var viewModel = new RecoveryViewModel();
                return View(viewModel);
            });
        }

        [HttpPost]
        public ActionResult Recovery(RecoveryViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = unitOfWork.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "UserName",
                        Value = viewModel.UserName
                    }
                }).FirstOrDefault();

                if (dbUser == null || dbUser.Email != viewModel.Email)
                {
                    viewModel.ErrorMessage = "I couldn't find your account information.";
                    return View(viewModel);
                }

                dbUser.Authenticated = false;
                dbUser.VerificationCode = Guid.NewGuid().ToString();
                unitOfWork.Users.Edit(dbUser);
                SetSessionUser(dbUser);

                //TODO: Send guid over email.
                return RedirectToAction("Verify");
            });
        }

        public JsonResult Update()
        {
            return HandleExceptions(() =>
            {
                var dbUser = GetSessionUserFromDb();
                var viewModel = new UpdateViewModel()
                {
                    UserName = dbUser.UserName,
                    Email = dbUser.Email
                };
                return Modal("_Update", viewModel);
            });
        }

        [HttpPost]
        public JsonResult Update(UpdateViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = GetSessionUserFromDb();

                if (dbUser.UserName != viewModel.UserName)
                {
                    var userNameCheck = unitOfWork.Users.Find(new Filter[]
                    {
                        new Filter()
                        {
                            Column = "UserName",
                            Value = viewModel.UserName
                        }
                    }).FirstOrDefault();

                    if (userNameCheck != null)
                        return Json(new { message = "That username is already in use chummer. Nothing was updated. Sorry." });

                    dbUser.UserName = viewModel.UserName;
                }

                if (dbUser.Email != viewModel.Email)
                {
                    var emailCheck = unitOfWork.Users.Find(new Filter[]
                    {
                        new Filter()
                        {
                            Column = "Email",
                            Value = viewModel.Email
                        }
                    }).FirstOrDefault();

                    if (emailCheck != null)
                        return Json(new { message = "That email is already in use chummer. Nothing was updated. Sorry." });

                    dbUser.Email = viewModel.Email;
                }

                if (viewModel.NewPassphrase != null && viewModel.ConfirmPassphrase != null && viewModel.NewPassphrase == viewModel.ConfirmPassphrase)
                    dbUser.PassPhrase = Sha512(viewModel.NewPassphrase + dbUser.CreatedOn);

                unitOfWork.Users.Edit(dbUser);
                SetSessionUser(dbUser);

                return Json(new
                {
                    message = "Your account has been uptated chummer, Have fun with your new SIN.",
                    html = dbUser.UserName
                });
            });
        }

        private void SetSessionUser(UserIdentity user)
        {
            var dbUserGroup = unitOfWork.UserGroups.GetById(user.GroupId).Name;
            HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(user));
            HttpContext.Session.SetString("UserGroup", dbUserGroup);
        }

        private UserIdentity GetSessionUser()
        {
            var jsonString = HttpContext.Session.GetString("UserKey");
            if (jsonString == null)
                throw new Exception("User session var is not set.");
            return JsonConvert.DeserializeObject<UserIdentity>(jsonString);
        }

        private UserIdentity GetSessionUserFromDb()
        {
            var user = GetSessionUser();
            return unitOfWork.Users.GetById(user.Id ?? 0);
        }
    }
}