using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using ThrowAwayProjects.Models;
using Microsoft.AspNetCore.Http;
using ThrowAwayData;
using Newtonsoft.Json;

namespace ThrowAwayProjects.Controllers
{
    public class ForumController : BaseController
    {
        public ForumController(ICompositeViewEngine viewEngine, IConfiguration configuration, IWebHostEnvironment environment) :
        base(viewEngine, configuration, environment)
        {
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

        [HttpPost]
        public ActionResult AddEditThread(ThreadViewModel viewModel)
        {
            return HandleExceptions(() =>
            {
                var key = HttpContext.Session.GetString("UserKey");

                if (key == null)
                    throw new Exception("You must be logged in to do that.");

                var user = JsonConvert.DeserializeObject<UserIdentity>(key);
                var sessionString = HttpContext.Session.GetString("CurrentEditId");
                var dbThread = new Thread();

                if (sessionString == null)
                {
                    dbThread.Title = viewModel.Title;
                    dbThread.Body = viewModel.Body;
                    dbThread.CreatedBy = user.Id ?? 0;
                    database.Threads.Add(dbThread);
                }
                else
                {
                    dbThread = database.Threads.Get(Convert.ToInt32(sessionString));
                    dbThread.Title = viewModel.Title;
                    dbThread.Body = viewModel.Body;
                    database.Threads.Edit(dbThread);
                }

                return RedirectToAction("Thread");
            });
        }
    }
}
