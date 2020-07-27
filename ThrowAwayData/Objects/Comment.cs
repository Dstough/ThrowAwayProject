using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Comment : BaseObject
    {
        public string Body { get; set; }
        public bool Edited { get; set; }
        public Comment() : base()
        {
            Body = "";
            Edited = false;
        }
    }
}
