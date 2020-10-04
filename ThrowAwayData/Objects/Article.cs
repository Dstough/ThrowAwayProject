using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class Article : BaseObject
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int TagId { get; set; }

        public Article() : base()
        {
            Title = "";
            Body = "";
            TagId = 1;
        }
    }
}
