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
using Newtonsoft.Json;
using ThrowAwayData;

namespace ThrowAwayProjects.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ICompositeViewEngine viewEngine, IConfiguration configuration, IHostingEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
        }

        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var date = DateTime.Now;
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("FirstSeen")))
                    HttpContext.Session.SetString("FirstSeen", JsonConvert.SerializeObject(date));
                else
                    date = JsonConvert.DeserializeObject<DateTime>(HttpContext.Session.GetString("FirstSeen"));

                var user = JsonConvert.DeserializeObject<UserIdentity>(HttpContext.Session.GetString(configuration.GetValue<string>("UserKey")));

                var model = new SessionStateViewModel
                {
                    DateSessionStarted = date,
                    Now = DateTime.Now,
                    AccountAge = (int)(DateTime.Now - user.CreatedOn).TotalDays
                };
                return View(model);
            });
        }
    }
}
