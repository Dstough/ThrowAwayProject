using System;
using System.Collections.Generic;
using ThrowAwayProjects.Models;
using ThrowAwayProjects.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using ThrowAwayDb;

namespace ThrowAwayProjects.Controllers
{
    public class TrainController : BaseController
    {
        public TrainController(ICompositeViewEngine viewEngine,IConfiguration configuration,IHostingEnvironment environment):
        base(viewEngine,configuration,environment)
        {
        }
        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var list = new List<TrainViewModel>();
                
                return View(list);
            });
        }

        public JsonResult AddEdit(int id)
        {
            return HandleExceptions(() =>
            {
                var viewModel = new TrainViewModel();

                return Modal("Partials/_AddEdit", viewModel);
            });
        }
        [HttpPost] public ActionResult AddEdit(TrainViewModel viewModel, string action)
        {
            return HandleExceptions(() =>
            {
                if (action == "Add New")
                {
                    
                }
                else if (action == "Edit Plane")
                {

                }
                else if(action == "Delete")
                {

                }
                return RedirectToAction("Index");
            });
        }
    }
}