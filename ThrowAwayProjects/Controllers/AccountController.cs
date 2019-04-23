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
                var defaultUserGroup = database.UserGroups.Where(new { Name = "User" }).Find().FirstOrDefault();
                var userNameCheck = database.Users.Where(new { UserName = viewModel.UserName }).Find().FirstOrDefault();
                var emailCheck = database.Users.Where(new { Email = viewModel.Email }).Find().FirstOrDefault();

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
                    UserGroupId = defaultUserGroup.Id ?? 0,
                    UserName = viewModel.UserName,
                    CreatedOn = CreatedDate,
                    Email = viewModel.Email,
                    Passphrase = Sha512(viewModel.Passphrase + CreatedDate),
                    VerificationCode = Guid.NewGuid().ToString()
                };
                database.Users.Add(user);
                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(user));
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

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));

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
                if (HttpContext.Session.GetString("UserKey") == null)
                    return RedirectToAction("Index", "Home");

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                var dbUser = database.Users.Include("UserGroup").Get(user.Id ?? 0);

                if (viewModel.inputGuid != dbUser.VerificationCode)
                    viewModel.ErrorMessage = "The verification code was not correct.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                dbUser.Authenticated = true;
                database.Users.Edit(dbUser);
                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(dbUser));
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
                var dbUser = database.Users.Where(new { Email = viewModel.Email }).Find().FirstOrDefault();

                if (dbUser == null)
                    viewModel.ErrorMessage = "I couldn't find your account information.";

                if (dbUser.VerificationCode != viewModel.VerificationCode)
                    viewModel.ErrorMessage = "The verification code was not correct.";

                if (viewModel.ErrorMessage != null)
                    return View(viewModel);

                dbUser.Authenticated = false;
                dbUser.VerificationCode = Guid.NewGuid().ToString();
                database.Users.Edit(dbUser);
                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(dbUser));
                //TODO: Send guid over email.
                return RedirectToAction("Verify");
            });
        }

        public JsonResult Update()
        {
            return HandleExceptions(() =>
            {
                if (HttpContext.Session.GetString("UserKey") == null)
                    throw new Exception("You must be logged in to update your account.");

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                var dbUser = database.Users.Include("UserGroup").Get(user.Id ?? 0);

                if (!dbUser.Authenticated)
                    throw new Exception("You must verify your email address before you can update your account.");

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
                if (HttpContext.Session.GetString("UserKey") == null)
                    throw new Exception("You must be logged in to update your account.");

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                var dbUser = database.Users.Include("UserGroup").Get(user.Id ?? 0);

                if (!dbUser.Authenticated)
                    throw new Exception("You must verify your email address before you can update your account.");

                if (dbUser.UserName != viewModel.UserName)
                {
                    var userNameCheck = database.Users.Where(new { UserName = viewModel.UserName }).Find().FirstOrDefault();

                    if (userNameCheck != null)
                        throw new Exception("That username is already in use chummer. Nothing was updated. Sorry.");

                    dbUser.UserName = viewModel.UserName;
                }

                if (viewModel.NewPassphrase != null)
                {
                    if (viewModel.NewPassphrase != viewModel.ConfirmPassphrase)
                        throw new Exception("The new passphrase didn't match. Nothing was updated. Sorry.");

                    dbUser.Passphrase = Sha512(viewModel.NewPassphrase + dbUser.CreatedOn);
                }

                database.Users.Edit(dbUser);
                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(dbUser));

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
                if (HttpContext.Session.GetString("UserKey") == null)
                    return RedirectToAction("Index", "Home");

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                var dbUser = database.Users.Include("UserGroup").Get(user.Id ?? 0);
                var viewModel = new ChangeEmailViewModel(dbUser);
                return View(viewModel);
            });
        }

        [HttpPost]
        public ActionResult ChangeEmail(ChangeEmailViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                if (HttpContext.Session.GetString("UserKey") == null)
                    return RedirectToAction("Index", "Home");

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                var dbUser = database.Users.Include("UserGroup").Get(user.Id ?? 0);

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
                HttpContext.Session.SetString("UserKey", JsonConvert.SerializeObject(dbUser));
                //TODO: Send code to new email.
                return RedirectToAction("Verify", "Account");
            });
        }
    }
}