using DataBase;
namespace ThrowAwayDb
{
    public class Train : BaseObject
    {
        public string Name {get;set;}
        public string Cargo { get; set; }
        public int Cars { get; set; }

        public Train() : base()
        {
            Name = "Pain Train";
            Cargo = "Pain (double dose)";
            Cars = 1;
        }
    }
}