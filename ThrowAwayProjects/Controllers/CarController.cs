using System;
using System.Collections.Generic;
using ThrowAwayProjects.Models;
using ThrowAwayProjects.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using ThrowAwayDb;

namespace ThrowAwayProjects.Controllers
{
    public class CarController : BaseController
    {
        public CarController(ICompositeViewEngine viewEngine) : base(viewEngine)
        {
        }
        public ActionResult Index()
        {
            return HandleExceptions(() =>
            {
                var list = new List<CarViewModel>();
                foreach (var item in unitOfWork.Cars.GetAll())
                {
                    list.Add(new CarViewModel(item));
                }
                return View(list);
            });
        }

        public JsonResult AddEdit(int id)
        {
            return HandleExceptions(() =>
            {
                var car = id == 0 ? new Car() : unitOfWork.Cars.GetById(id);
                var viewModel = new CarViewModel(car);

                return Modal("Partials/_AddEdit", viewModel);
            });
        }
        [HttpPost] public ActionResult AddEdit(CarViewModel viewModel, string action)
        {
            return HandleExceptions(() =>
            {
                if (action == "Add New")
                {
                    var newItem = new Car()
                    {
                        Make = viewModel.Make,
                        Model = viewModel.Type,
                        CreatedBy = "WebUser",
                    };
                    unitOfWork.Cars.Add(newItem);
                }
                else if (action == "Edit Car")
                {
                    var item = unitOfWork.Cars.GetById(viewModel.Id);
                    item.Make = viewModel.Make;
                    item.Model = viewModel.Type;
                    unitOfWork.Cars.Edit(item);
                }
                else if(action == "Delete")
                {
                    unitOfWork.Cars.Delete(viewModel.Id);
                }
                return RedirectToAction("Index");
            });
        }
    }
}