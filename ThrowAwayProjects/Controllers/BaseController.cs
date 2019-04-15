using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public class BaseController : Controller
    {
        protected Database database;
        protected IConfiguration configuration;
        private ICompositeViewEngine viewEngine;
        private IHostingEnvironment environment;
        private ProcessStartInfo python;

        public BaseController(ICompositeViewEngine _viewEngine, IConfiguration _config, IHostingEnvironment _environment)
        {
            configuration = _config;
            viewEngine = _viewEngine;
            environment = _environment;
            database = new Database(configuration.GetConnectionString("ThrowAwayDB"));
            python = new ProcessStartInfo
            {
                FileName = "Python",
                Arguments = "",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
        }

        #region Core

        protected virtual ActionResult HandleExceptions(Func<ActionResult> logic)
        {
            try
            {
                ViewBag.Admin = false;
                ViewBag.LoggedIn = false;
                ViewBag.UserName = "Unknown";
                ViewBag.Privilege = "Unknown";
                ViewBag.Status = "Unknown";

                if (HttpContext.Session.GetString("SessionDate") == null)
                    HttpContext.Session.SetString("SessionDate", JsonConvert.SerializeObject(DateTime.Now.AddYears(60)));

                ViewBag.DateConnected = JsonConvert.DeserializeObject<DateTime>(HttpContext.Session.GetString("SessionDate"));

                if (HttpContext.Session.GetString("UserKey") != null)
                {
                    var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString("UserKey"));
                    ViewBag.LoggedIn = true;
                    ViewBag.Admin = user.UserGroup.Name == "Admin";
                    ViewBag.UserName = user.UserName;
                    ViewBag.Status = !user.Banned ? !user.Dead ? user.Authenticated ? "Active" : "Not Authenticated" : "Deceased" : "Banned";
                    ViewBag.Privilege = user.UserGroup.Name;
                }

                return logic();
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        protected virtual JsonResult HandleExceptions(Func<JsonResult> logic)
        {
            try
            {
                return logic();
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message });
            }
        }

        protected JsonResult Modal(string viewName, object model)
        {
            return Json(new { result = "modal", message = RenderViewToString(viewName, model) });
        }

        private string RenderViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;
            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                ViewEngineResult viewResult = viewEngine.FindView(ControllerContext, viewName, false);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, writer, new HtmlHelperOptions());

                var t = viewResult.View.RenderAsync(viewContext);
                t.Wait();

                return writer.GetStringBuilder().ToString();
            }
        }

        #endregion

        #region extra

        protected bool IsAuthenticated()
        {
            var value = HttpContext.Session.GetString("UserKey");

            if (string.IsNullOrEmpty(value))
                return false;

            var user = JsonConvert.DeserializeObject<UserIdentity>(value);

            if (user.Id == null)
                return false;

            var dbUserData = database.Users.Get(user.Id ?? 0);

            if (user.Passphrase != dbUserData.Passphrase)
                return false;

            return true;
        }

        protected string RunPythonScrit(dynamic script)
        {
            python.Arguments = environment.WebRootPath + "\\" + configuration.GetValue<string>("ServerScriptFolder") + "\\" + script.name + " ";
            foreach (var argument in script.arguments)
                python.Arguments += argument + " ";
            using (var process = Process.Start(python))
            using (var reader = process.StandardOutput)
                return reader.ReadToEnd();
        }

        protected static string Sha512(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var hashedInputStringBuilder = new StringBuilder(128);

                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));

                return hashedInputStringBuilder.ToString();
            }
        }

        #endregion
    }
}