using System.Collections.Generic;
using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Thread : BaseObject
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Edited { get; set; }
        public bool Closed { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public IEnumerable<Post> Post { get; set; }

        public Thread()
        {
            Title = "";
            Body = "";
            Edited = false;
            Closed = false;
            TagId = 0;
            Tag = new Tag();
            Post = new List<Post>();
        }
    }
}
