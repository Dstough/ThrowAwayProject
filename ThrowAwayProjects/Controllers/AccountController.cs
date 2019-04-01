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
        public AccountController(ICompositeViewEngine viewEngine, IConfiguration configuration, IHostingEnvironment environment) : base(viewEngine, configuration, environment)
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
                        Value = viewModel.UserName
                    }
                }).FirstOrDefault();

                if (dbUser != null && Sha512(viewModel.PassPhrase + dbUser.CreatedOn) == dbUser.PassPhrase)
                {
                    var dbUserGroup = unitOfWork.UserGroups.GetById(dbUser.GroupId).Name;
                    if (!dbUser.Authenticated)
                    {
                        //TODO: Send Auth Code to email.
                        var model = new VerificationViewModel()
                        {
                            UserId = dbUser.Id,
                            dbGuid = dbUser.VerificationCode
                        };
                        return View("Verify", model);
                    }
                    HttpContext.Session.SetString(configuration.GetValue<string>("UserKey"), JsonConvert.SerializeObject(dbUser));
                    HttpContext.Session.SetString("UserGroup", dbUserGroup);
                    return RedirectToAction(viewModel.TargetAction, viewModel.TargetController);
                }
                else
                    return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult LogOut()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("LogIn");
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
                var model = new UserViewModel();
                return View(model);
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

                if(defaultUserGroup == null)
                    viewModel.ErrorMessage = "We can't let you do that Omae.";

                if (emailCheck != null)
                    viewModel.ErrorMessage = "That email is already in use.";

                if (userNameCheck != null)
                    viewModel.ErrorMessage = "That username is already taken.";

                if(viewModel.PassPhrase != viewModel.PassPhraseConfirm)
                    viewModel.ErrorMessage = "Your passphrases didn't match.";
                
                if(viewModel.ErrorMessage != null)
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

                //TODO: Send Auth Code to email.
                var model = new VerificationViewModel()
                {
                    UserId = user.Id,
                    dbGuid = user.VerificationCode
                };
                return View("Verify", model);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        [HttpPost]
        public ActionResult Verify(VerificationViewModel viewModel)
        {
            try
            {
                var dbUser = unitOfWork.Users.GetById(viewModel.UserId ?? 0);
                var dbUserGroup = unitOfWork.UserGroups.GetById(dbUser.GroupId)?.Name;
                if (viewModel.inputGuid == dbUser.VerificationCode)
                {
                    dbUser.Authenticated = true;
                    unitOfWork.Users.Edit(dbUser);
                    HttpContext.Session.SetString(configuration.GetValue<string>("UserKey"), JsonConvert.SerializeObject(dbUser));
                    HttpContext.Session.SetString("UserGroup", dbUserGroup);
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("LogIn");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult Recovery()
        {
            try
            {
                var viewModel = new RecoveryViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        [HttpPost]
        public ActionResult Recovery(RecoveryViewModel viewModel)
        {
            try
            {
                var dbUser = unitOfWork.Users.Find(new Filter[]
                {
                    new Filter()
                    {
                        Column = "UserName",
                        Value = viewModel.UserName
                    }
                }).FirstOrDefault();

                if (dbUser.Email != viewModel.Email)
                    return View(viewModel);

                dbUser.Authenticated = false;
                dbUser.VerificationCode = Guid.NewGuid().ToString();
                unitOfWork.Users.Edit(dbUser);

                //TODO: Send guid over email.

                var model = new VerificationViewModel()
                {
                    UserId = dbUser.Id,
                    dbGuid = dbUser.VerificationCode
                };
                return View("Verify", model);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
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
                return Modal("Update", viewModel);
            });
        }

        [HttpPost]
        public ActionResult Update(UpdateViewModel viewModel)
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
                HttpContext.Session.SetString(configuration.GetValue<string>("UserKey"), JsonConvert.SerializeObject(dbUser));
                return Json(new { message = "Your account has been uptated chummer, Have fun with your new SIN." });
            });
        }

        private UserIdentity GetSessionUserFromDb()
        {
            var userKey = configuration.GetValue<string>("UserKey");
            var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString(userKey));
            return unitOfWork.Users.GetById(user.Id ?? 0);
        }
    }
}