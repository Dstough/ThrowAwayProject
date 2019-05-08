using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Post : BaseObject
    {
        public int ThreadId { get; set; }
        public Thread Thread { get; set; }
        public string Body { get; set; }
        public bool Edited { get; set; }

        public Post() : base()
        {
            Body = "";
            Edited = false;
            ThreadId = 0;
            Thread = null;
        }
    }
}