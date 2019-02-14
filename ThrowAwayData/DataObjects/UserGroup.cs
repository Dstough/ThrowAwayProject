using ThrowAwayDataBackground;
namespace ThrowAwayDataBackground
{
    public class UserGroup : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public UserGroup() : base()
        {
            Name = null;
            Description = null;
        }
    }
}