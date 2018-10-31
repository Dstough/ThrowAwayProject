using System;
using System.Collections.Generic;
using ThrowAwayProjects.Models;
using ThrowAwayProjects.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace ThrowAwayControllers
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
                foreach(var item in unitOfWork.Cars.GetAll())
                {
                    list.Add(new CarViewModel(item));
                }
                return View(list);
            });
        }
        public JsonResult Edit(int id)
        {
            return HandleExceptions(() =>
            {
                var car = unitOfWork.Cars.GetById(id);
                var viewModel = new CarViewModel(car);
                return Modal("Partials/_Edit", viewModel);
            });
        }
        [HttpPost]
        public ActionResult Edit(CarViewModel viewModel)
        {
            return HandleExceptions(()=>
            {
                return RedirectToAction("Index");
            });
        }
    }
}