using ThrowAwayDb;
namespace ThrowAwayProjects.Models
{
    public class CarViewModel
    {
        public string Make { get; private set; }
        public string CarModel { get; private set; }
        public int Id {get;set;}
        public CarViewModel() : this(new Car())
        {

        }
        public CarViewModel(Car _Car)
        {
            Id = _Car.Id;
            Make = _Car.Make;
            CarModel = _Car.Model;
        }
    }
}