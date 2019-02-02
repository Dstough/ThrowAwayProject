using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class CarViewModel
    {
        public string Make { get; set; }
        public string Type { get; set; }
        public int Id {get;set;}
        public CarViewModel() : this(new Car())
        {

        }
        public CarViewModel(Car _Car)
        {
            Id = _Car.Id;
            Make = _Car.Make;
            Type = _Car.Model;
        }
    }
}