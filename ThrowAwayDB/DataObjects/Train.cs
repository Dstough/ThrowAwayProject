using DataBase;
namespace ThrowAwayDb
{
    public class Train : BaseObject
    {
        public string Name {get;set;}
        public string Cargo { get; set; }
        public int? Cars { get; set; }

        public Train() : base()
        {
            Name = null;
            Cargo = null;
            Cars = null;
        }
    }
}