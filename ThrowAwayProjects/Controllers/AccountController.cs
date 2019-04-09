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
                var defaultUserGroup = database.UserGroups.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "Name",
                        Value = "User"
                    }
                }).FirstOrDefault();

                var userNameCheck = database.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "UserName",
                        Value = viewModel.UserName
                    }
                }).FirstOrDefault();

                var emailCheck = database.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "Email",
                        Value = viewModel.Email
                    }
                }).FirstOrDefault();

                if (defaultUserGroup == null)
                    viewModel.ErrorMessage = "Something went horribly wrong chummer. Try clearing your cache and re-lode the page.";

                if (emailCheck != null)
                    viewModel.ErrorMessage = "That email is already in use.";

                if (userNameCheck != null)
                    viewModel.ErrorMessage = "That username is already taken.";

                if (viewModel.Passphrase != viewModel.PassphraseConfirm)
                    viewModel.ErrorMessage = "Your Passphrases didn't match.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                var user = new UserIdentity
                {
                    GroupId = defaultUserGroup.Id ?? 0,
                    UserName = viewModel.UserName,
                    CreatedOn = CreatedDate,
                    Email = viewModel.Email,
                    Passphrase = Sha512(viewModel.Passphrase + CreatedDate),
                    VerificationCode = Guid.NewGuid().ToString()
                };
                database.Users.Add(user);
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

                if (user.Authenticated)
                    return RedirectToAction("Index", "Home");

                var model = new VerificationViewModel()
                {
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
                var dbUser = GetSessionUserFromDb();

                if (viewModel.inputGuid != dbUser.VerificationCode)
                    viewModel.ErrorMessage = "The verification code was not correct.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                dbUser.Authenticated = true;
                database.Users.Edit(dbUser);
                SetSessionUser(dbUser);
                return RedirectToAction("Index", "Home");
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
                var dbUser = database.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "Email",
                        Value = viewModel.Email
                    }
                }).FirstOrDefault();

                if (dbUser == null)
                    viewModel.ErrorMessage = "I couldn't find your account information.";

                if (dbUser.VerificationCode != viewModel.VerificationCode)
                    viewModel.ErrorMessage = "The verification code was not correct.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                dbUser.Authenticated = false;
                dbUser.VerificationCode = Guid.NewGuid().ToString();
                database.Users.Edit(dbUser);
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

                if (!dbUser.Authenticated) return Json(new
                {
                    result = "error",
                    message = "You must verify your email address before you can update your account."
                });

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
                if (!dbUser.Authenticated) return Json(new
                {
                    result = "error",
                    message = "You must verify your email address before you can update your account."
                });

                if (dbUser.UserName != viewModel.UserName)
                {
                    var userNameCheck = database.Users.Find(new Filter[]
                    {
                        new Filter()
                        {
                            Column = "UserName",
                            Value = viewModel.UserName
                        }
                    }).FirstOrDefault();

                    if (userNameCheck != null) return Json(new
                    {
                        result = "error",
                        message = "That username is already in use chummer. Nothing was updated. Sorry."
                    });

                    dbUser.UserName = viewModel.UserName;
                }

                if (viewModel.NewPassphrase != null)
                {
                    if (viewModel.NewPassphrase != viewModel.ConfirmPassphrase)
                        return Json(new
                        {
                            result = "error",
                            message = "The new passphrase didn't match. Nothing was updated. Sorry."
                        });
                    dbUser.Passphrase = Sha512(viewModel.NewPassphrase + dbUser.CreatedOn);
                }

                database.Users.Edit(dbUser);
                SetSessionUser(dbUser);

                return Json(new
                {
                    message = "Your account has been uptated chummer, Have fun with your new SIN.",
                    html = dbUser.UserName
                });
            });
        }

        public ActionResult ChangeEmail()
        {
            return HandleExceptions(() =>
            {
                var dbUser = GetSessionUserFromDb();
                var viewModel = new ChangeEmailViewModel(dbUser);
                return View(viewModel);
            });
        }

        [HttpPost]
        public ActionResult ChangeEmail(ChangeEmailViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var dbUser = GetSessionUserFromDb();

                if (viewModel.NewEmail == null)
                    viewModel.ErrorMessage = "You have to type a new email for me to change it omae.";

                if (viewModel.AuthenticationCode == null || viewModel.AuthenticationCode != dbUser.VerificationCode)
                    viewModel.ErrorMessage = "Your authentication code wasn't valid chummer, try again.";

                if (viewModel.Passphrase == null || Sha512(viewModel.Passphrase + dbUser.CreatedOn) != dbUser.Passphrase)
                    viewModel.ErrorMessage = "Your passphrase didn't match chummer, try again.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                dbUser.Email = viewModel.NewEmail;
                dbUser.Authenticated = false;
                dbUser.VerificationCode = Guid.NewGuid().ToString();
                database.Users.Edit(dbUser);
                SetSessionUser(dbUser);
                //TODO: Send code to new email.
                return RedirectToAction("Verify", "Account");
            });
        }

        private void SetSessionUser(UserIdentity user)
        {
            var dbUserGroup = database.UserGroups.GetById(user.GroupId).Name;
            HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(user));
            HttpContext.Session.SetString("UserGroup", dbUserGroup);
        }

        private UserIdentity GetSessionUser()
        {
            var jsonString = HttpContext.Session.GetString("UserKey");
            if (jsonString == null)
                throw new Exception("You will have to log in to continue.");
            return JsonConvert.DeserializeObject<UserIdentity>(jsonString);
        }

        private UserIdentity GetSessionUserFromDb()
        {
            var user = GetSessionUser();
            return database.Users.GetById(user.Id ?? 0);
        }
    }
}