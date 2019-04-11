using ThrowAwayDataBackground;
namespace ThrowAwayDataBackground
{
    public class Post : BaseObject
    {
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public bool Closed { get; set; }
        
        public Post() : base()
        {
            ParentId = null;
            Title = "";
            Body = "";
            Tags = "";
            Closed = false;
        }
    }
}