using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class UserGroup : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Style { get; set; }

        public UserGroup() : base()
        {
            Name = "";
            Description = "";
            Style = "";
        }
    }
}