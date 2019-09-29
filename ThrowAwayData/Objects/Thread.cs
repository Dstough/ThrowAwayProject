using System.Collections.Generic;
using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Thread : BaseObject
    {
        public string PublicId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Edited { get; set; }
        public bool Closed { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public IEnumerable<Post> Post { get; set; }
    }
}
