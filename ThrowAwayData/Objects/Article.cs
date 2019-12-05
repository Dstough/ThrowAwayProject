using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    class Article : BaseObject
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public Article() : base()
        {
            Title = "";
            Body = "";
        }
    }
}
