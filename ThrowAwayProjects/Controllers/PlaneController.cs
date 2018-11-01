using System;
using System.Collections.Generic;
using ThrowAwayProjects.Models;
using ThrowAwayProjects.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using ThrowAwayDb;

namespace ThrowAwayProjects.Controllers
{
    public class PlaneController : BaseController
    {
        public PlaneController(ICompositeViewEngine viewEngine) : base(viewEngine)
        {
        }
        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var list = new List<PlaneViewModel>();
                foreach (var item in unitOfWork.Planes.GetAll())
                {
                    list.Add(new PlaneViewModel(item));
                }
                return View(list);
            });
        }

        public JsonResult AddEdit(int id)
        {
            return HandleExceptions(() =>
            {
                var plane = id == 0 ? new Plane() : unitOfWork.Planes.GetById(id);
                var viewModel = new PlaneViewModel(plane);

                return Modal("Partials/_AddEdit", viewModel);
            });
        }
        [HttpPost] public ActionResult AddEdit(PlaneViewModel viewModel, string action)
        {
            return HandleExceptions(() =>
            {
                if (action == "Add New")
                {
                    var newItem = new Plane()
                    {
                        Name = viewModel.Name,
                        Cabin = viewModel.Cabin,
                        MaxPassengerCount = viewModel.MaxPassengerCount,
                        CreatedBy = "WebUser",
                    };
                    unitOfWork.Planes.Add(newItem);
                }
                else if (action == "Edit Plane")
                {
                    var item = unitOfWork.Planes.GetById(viewModel.Id);
                    item.Name = viewModel.Name;
                    item.Cabin = viewModel.Cabin;
                    item.MaxPassengerCount = viewModel.MaxPassengerCount;
                    item.CreatedBy = "WebUser";
                    unitOfWork.Planes.Edit(item);
                }
                else if(action == "Delete")
                {
                    unitOfWork.Planes.Delete(viewModel.Id);
                }
                return RedirectToAction("Index");
            });
        }
    }
}