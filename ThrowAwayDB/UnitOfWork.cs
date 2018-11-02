using System;
using ThrowAwayDbBackground;
namespace ThrowAwayDB
{
    public class UnitOfWork
    {
        public CarRepository Cars {get;set;}
        public PlaneRepository Planes {get;set;}

        public UnitOfWork(): this("") 
        { }
        public UnitOfWork(string ConnectionString)
        {
            Cars = new CarRepository(ConnectionString);
            Planes = new PlaneRepository(ConnectionString);
        }
    }
}
