using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using Microsoft.AspNetCore.Http;

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

        public JsonResult AddEditThread(int Id)
        {
            return HandleExceptions(() => 
            {
                HttpContext.Session.SetString("CurrentEditId", Id.ToString());

                if (Id == 0)
                    return Modal("_AddEditThread", new ThreadViewModel());

                var DBthread = database.Threads.Get(Id);

                if (DBthread == null)
                    throw new Exception("That thread couldn't be found in the database.");
                
                var viewModel = new ThreadViewModel()
                {
                    Title = DBthread.Title,
                    Body = DBthread.Body
                };

                return Modal("_AddEditThread", viewModel);
            });
        }
    }
}
