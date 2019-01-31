using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ThrowAwayDB;

namespace ThrowAwayProjects.Controllers
{
    public class BaseController : Controller
    {
        protected UnitOfWork unitOfWork;
        private ICompositeViewEngine viewEngine;
        private IConfiguration configuration;
        private IHostingEnvironment environment;
        private ProcessStartInfo python;

        public BaseController(ICompositeViewEngine _viewEngine, IConfiguration _config, IHostingEnvironment _environment)
        {
            configuration = _config;
            viewEngine = _viewEngine;
            environment = _environment;
            unitOfWork = new UnitOfWork(configuration.GetConnectionString("ThrowAwayDB"));
            python = new ProcessStartInfo
            {
                FileName = "Python",
                Arguments = "",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
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
        public ActionResult HandleExceptions(Func<ActionResult> logic)
        {
            try
            {
                return logic();
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }
        public JsonResult HandleExceptions(Func<JsonResult> logic)
        {
            try
            {
                return logic();
            }
            catch (Exception ex)
            {
                return Modal("ModalError", ex);
            }
        }
        protected JsonResult Modal(string viewName, object model)
        {
            return Json(new { message = RenderViewToString(viewName, model) });
        }
        protected String RunPythonScrit(dynamic script)
        {
            python.Arguments = environment.WebRootPath + "\\" + configuration.GetValue<string>("ServerScriptFolder") + "\\" + script.name + " ";
            foreach (var argument in script.arguments)
                python.Arguments += argument + " ";
            using (var process = Process.Start(python))
            using (var reader = process.StandardOutput)
            {
                return reader.ReadToEnd();
            }
        }
    }
}