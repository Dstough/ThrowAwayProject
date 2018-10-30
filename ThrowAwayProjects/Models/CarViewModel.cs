using ThrowAwayDb;
namespace ThrowAwayProjects.Models
{
    public class CarViewModel
    {
        public string Make { get; private set; }
        public string Model { get; private set; }
        public int Id {get;set;}
        public CarViewModel() :this(new Car())
        {
            
        }
        public CarViewModel(Car _Car)
        {
            Id = _Car.Id ?? 0;
            Make = _Car.Make;
            Model = _Car.Model;
        }
    }
}