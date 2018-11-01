using System;
using ThrowAwayDbBackground;
namespace ThrowAwayDB
{
    public class UnitOfWork
    {
        public CarRepository Cars {get;set;}
        public PlaneRepository Planes {get;set;}

        public UnitOfWork()
        {
            Cars = new CarRepository();
            Planes = new PlaneRepository();
        }
    }
}
