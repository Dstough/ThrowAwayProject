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
                const string sessionKey = "FirstSeen";
                DateTime dateFirstSeen;
                var value = HttpContext.Session.GetString(sessionKey);

                if (string.IsNullOrEmpty(value))
                {
                    dateFirstSeen = DateTime.Now;
                    var serialisedDate = JsonConvert.SerializeObject(dateFirstSeen);
                    HttpContext.Session.SetString(sessionKey, serialisedDate);
                }
                else
                {
                    dateFirstSeen = JsonConvert.DeserializeObject<DateTime>(value);
                }

                var model = new SessionStateViewModel
                {
                    DateSessionStarted = dateFirstSeen,
                    Now = DateTime.Now
                };

                var x = RunPythonScrit(new
                {
                    name = "DiceRoll.py",
                    arguments = new List<string>() { "1", "13" }
                });

                return View(model);
            });
        }
    }
}
