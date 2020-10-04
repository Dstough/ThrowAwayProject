using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
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

        public ActionResult Index(int publicId = 0)
        {
            return HandleExceptions(() =>
            {
                var dbUsers = database.Users.GetPage((publicId), 50);
                var viewModel = new UserListViewModel()
                {
                    page = publicId
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

        public JsonResult AddEdit(string publicId = "")
        {
            return HandleExceptions(() =>
            {
                var groups = database.UserGroups.GetAll();
                var viewModel = new UserViewModel();

                foreach (var item in groups)
                    viewModel.GroupOptions.Add(new SelectListItem(item.Name, item.Id.ToString()));

                if (publicId == null)
                    return Modal("_AddEdit", viewModel);
                
                HttpContext.Session.SetString("CurrentEditId", publicId);

                var dbUser = database.Users.Where(new { PublicId = publicId }).Find().FirstOrDefault();

                if (dbUser == null)
                    throw new Exception("User not found in database");

                var group = database.UserGroups.Get(dbUser.UserGroupId);

                viewModel.GroupOptions.Where(e => e.Value == group.Id.ToString()).First().Selected = true;
                viewModel.PublicId = dbUser.PublicId;
                viewModel.GroupId = dbUser.UserGroupId;
                viewModel.UserName = dbUser.UserName;
                viewModel.Email = dbUser.Email;
                viewModel.Authenticated = dbUser.Authenticated;
                viewModel.Banned = dbUser.Banned;
                viewModel.Dead = dbUser.Dead;
                viewModel.GroupName = group.Name;

                return Modal("_AddEdit", viewModel);
            });
        }

        [HttpPost]
        public JsonResult AddEdit(UserViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var key = HttpContext.Session.GetString("CurrentEditId");
                var result = "edit";

                if (key == null)
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var publicId = key;

                viewModel.PublicId = publicId;
                HttpContext.Session.SetString("CurrentEditId", 0.ToString());

                var dbUser = publicId == null ? new UserIdentity() : database.Users.Where(new { PublicId = publicId }).Find().FirstOrDefault();

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
                {
                    database.Users.Add(dbUser);
                    result = "prepend";
                }
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
                    result,
                    newId = dbUser.Id,
                    html = htmlString
                });
            });
        }

        [HttpPost]
        public JsonResult Delete()
        {
            return HandleExceptions(() =>
            {
                var publicId = HttpContext.Session.GetString("CurrentEditId");

                if (publicId == null)
                    throw new Exception("Stop poking around where you shouldn't be omae.");

                var user = database.Users.Where(new { PublicId = publicId }).Find().FirstOrDefault();

                if (user == null)
                    throw new Exception("user not found in the database.");

                database.Users.Delete(user.Id ?? 0);

                return Json(new
                {
                    message = "The user was removed"
                });
            });
        }
    }
}