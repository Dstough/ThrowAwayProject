using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using ThrowAwayData;

namespace ThrowAwayProjects.Controllers
{
    public class ForumController : BaseController
    {
        public ForumController(ICompositeViewEngine viewEngine, IConfiguration configuration, IWebHostEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var model = new List<PostViewModel>();
                return View();
            });
        }

        public ActionResult NewThread()
        {
            return HandleExceptions(() => 
            {

                return View();
            });
        }
    }
}
