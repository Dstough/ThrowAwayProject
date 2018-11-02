using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public BaseController(ICompositeViewEngine _viewEngine,IConfiguration _config)
        {
            configuration = _config;
            viewEngine = _viewEngine;
            unitOfWork = new UnitOfWork(configuration.GetConnectionString("ThrowAwayDB"));
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
    }
}