using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Post : BaseObject
    {
        public string Body { get; set; }
        public bool Edited { get; set; }
        public int ThreadId { get; set; }

        public Post() : base()
        {
            Body = "";
            Edited = false;
            ThreadId = 0;
        }
    }
}