using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThrowAwayProjects.Controllers
{
    public class ArticleController : BaseController
    {
        public ArticleController(ICompositeViewEngine _viewEngine, IConfiguration _config, IWebHostEnvironment _environment) : base(_viewEngine, _config, _environment)
        {}

        public ActionResult Article(string Id)
        {
            
            return View();
        }
    }
}
