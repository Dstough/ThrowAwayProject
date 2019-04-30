using System;
using System.Collections.Generic;
using System.Linq;
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
        public UserController(ICompositeViewEngine viewEngine, IConfiguration configuration, IWebHostEnvironment environment) :
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

                if (database.UserGroups.Get(user.UserGroupId).Name != "Admin")
                    return RedirectToAction("Index", "Home");

                return base.HandleExceptions(logic);
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
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var key = HttpContext.Session.GetString("UserKey");
                var user = JsonConvert.DeserializeObject<UserIdentity>(key);

                if (database.UserGroups.Get(user.UserGroupId).Name != "Admin")
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                return logic();
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }

        public ActionResult Index(int? id)
        {
            return HandleExceptions(() =>
            {
                if (id == null)
                    throw new Exception("That set isn't in the database chummer. Try again.");

                var dbUsers = database.Users.GetPage((id ?? 0) + 1, 50);
                var viewModel = new UserListViewModel()
                {
                    page = id ?? 0
                };

                foreach (var user in dbUsers)
                {
                    viewModel.list.Add(new UserViewModel(user)
                    {
                        GroupName = database.UserGroups.Get(user.UserGroupId).Name
                    });
                }

                return View(viewModel);
            });
        }

        public JsonResult AddEdit(int? Id)
        {
            return HandleExceptions(() =>
            {
                HttpContext.Session.SetString("CurrentEditId", Id.ToString());

                var groups = database.UserGroups.GetAll();
                var viewModel = new UserViewModel();

                foreach (var item in groups)
                    viewModel.GroupOptions.Add(new SelectListItem(item.Name, item.Id.ToString()));

                if (Id == 0)
                    return Modal("_AddEdit", viewModel);

                var dbUser = database.Users.Include("UserGroup").Get(Id ?? 0);

                viewModel.GroupOptions.Where(e => e.Value == dbUser.UserGroup.Id.ToString()).First().Selected = true;
                viewModel.Id = dbUser.Id ?? 0;
                viewModel.GroupId = dbUser.UserGroupId;
                viewModel.UserName = dbUser.UserName;
                viewModel.Email = dbUser.Email;
                viewModel.Authenticated = dbUser.Authenticated;
                viewModel.Banned = dbUser.Banned;
                viewModel.Dead = dbUser.Dead;
                viewModel.GroupName = dbUser.UserGroup.Name;

                return Modal("_AddEdit", viewModel);
            });
        }

        [HttpPost]
        public JsonResult AddEdit(UserViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var key = HttpContext.Session.GetString("CurrentEditId");

                if (key == null)
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var Id = Convert.ToInt32(key);

                viewModel.Id = Id;
                HttpContext.Session.SetString("CurrentEditId", 0.ToString());

                var dbUser = Id == 0 ? new UserIdentity() : database.Users.Get(Id);

                if (dbUser.UserName != viewModel.UserName)
                    dbUser.UserName = viewModel.UserName;

                if (dbUser.Email != viewModel.Email)
                    dbUser.Email = viewModel.Email;

                if (dbUser.UserGroupId != viewModel.GroupId)
                    dbUser.UserGroupId = viewModel.GroupId;

                if (viewModel.Passphrase != null)
                    dbUser.Passphrase = Sha512(viewModel.Passphrase + dbUser.CreatedOn);

                if (dbUser.Authenticated != viewModel.Authenticated)
                    dbUser.Authenticated = viewModel.Authenticated;

                if (dbUser.Banned != viewModel.Banned)
                    dbUser.Banned = viewModel.Banned;

                if (dbUser.Dead != viewModel.Dead)
                    dbUser.Dead = viewModel.Dead;

                if (dbUser.Id == null)
                    database.Users.Add(dbUser);
                else
                    database.Users.Edit(dbUser);

                var group = database.UserGroups.Get(dbUser.UserGroupId);
                var htmlString = "<td>" + dbUser.UserName + "</td>";
                htmlString += "<td>" + dbUser.Email + "</td>";
                htmlString += "<td>" + group.Name + "</td>";
                htmlString += "<td>" + dbUser.Authenticated + "</td>";
                htmlString += "<td>" + dbUser.Banned + "</td>";
                htmlString += "<td>" + dbUser.Dead + "</td>";

                return Json(new
                {
                    html = htmlString
                });
            });
        }

        [HttpPost]
        public JsonResult Delete()
        {
            return HandleExceptions(() =>
            {
                var key = HttpContext.Session.GetString("CurrentEditId");

                if (key == null)
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var Id = Convert.ToInt32(key);

                database.Users.Delete(Id);

                return Json(new
                {
                    html = "<td colspan='6'>&nbsp;</td>"
                });
            });
        }
    }
}