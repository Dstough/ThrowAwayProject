using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;

namespace ThrowAwayProjects.Controllers
{
    public class LogInController : BaseController
    {
        public LogInController(ICompositeViewEngine viewEngine, IConfiguration configuration, IHostingEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        [HttpPost]
        public ActionResult Authenticate(UserViewModel viewModel)
        {
            try
            {
                if (IsAuthenticated())
                    return Redirect(viewModel.TargetUrl);
                else
                    return View("LogIn", viewModel);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }
    }
}