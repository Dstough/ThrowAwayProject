using System;
namespace DataBase
{
    public abstract class BaseObject
    {
        public int? Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public BaseObject()
        {
            Id = null;
            CreatedOn = DateTime.Now;
            CreatedBy = "System";
            Deleted = false;
        }
    }
}