using System;
namespace ThrowAwayDataBackground
{
    public abstract class BaseObject
    {
        public int? Id { get; set; }
        public string PublicId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public bool Deleted { get; set; }

        public BaseObject()
        {
            Id = null;
            PublicId = null;
            CreatedOn = DateTime.Now;
            CreatedBy = 0;
            Deleted = false;
        }
    }
}