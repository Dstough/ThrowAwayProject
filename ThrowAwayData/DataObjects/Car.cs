using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class Car : BaseObject
    {
        public string Make { get; set; }
        public string Model { get; set; }

        public Car() : base()
        {
            Make = null;
            Model = null;
        }
    }
}