using DataBase;
namespace ThrowAwayDb
{
    public class Plane : BaseObject
    {
        public string Name {get;set;}
        public string Cabin { get; set; }
        public int MaxPassengerCount { get; set; }

        public Plane() : base()
        {
            Name = "Planey mc Plane Face";
            Cabin = "Large";
            MaxPassengerCount = 0;
        }
    }
}