using DataBase;
namespace ThrowAwayDb
{
    public class Car : BaseObject
    {
        public string Make { get; set; }
        public string Model { get; set; }

        public Car() : base()
        {
            Make = "GM";
            Model = "Default";
        }
    }
}