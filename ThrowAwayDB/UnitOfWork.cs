using System;
using DataBase;
namespace ThrowAwayDB
{
    public class UnitOfWork
    {
        public CarRepository Cars {get;set;}
        public PlaneRepository Planes {get;set;}
        public TrainRepository Trains {get;set;}

        public UnitOfWork()
        {
            Cars = new CarRepository();
            Planes = new PlaneRepository();
            Trains = new TrainRepository();
        }
    }
}
