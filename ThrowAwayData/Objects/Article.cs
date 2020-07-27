using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Article : BaseObject
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Category { get; set; }
        public byte[] Image { get; set; }

        public Article() : base()
        {
            Title = "";
            Body = "";
            Category = "";
            Image = null;
        }
    }
}
