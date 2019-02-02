using System;
namespace ThrowAwayDataBackground
{
    public abstract class BaseObject
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public BaseObject()
        {
            Id = 0;
            CreatedOn = DateTime.Now;
            CreatedBy = "System";
            Deleted = false;
        }
    }
}