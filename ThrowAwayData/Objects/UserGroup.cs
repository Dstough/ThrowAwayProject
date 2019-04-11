using ThrowAwayDataBackground;
using System.Collections.Generic;
namespace ThrowAwayData
{
    public class UserGroup : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<UserIdentity> UserIdentity { get; set; }

        public UserGroup() : base()
        {
            Name = null;
            Description = null;
            UserIdentity = null;
        }
    }
}