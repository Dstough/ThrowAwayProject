using System;
using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class Database
    {
        public UserGroupRepository UserGroups { get; private set; }
        public UserIdentityRepository Users { get; private set; }

        public Database() : this("")
        {
        }

        public Database(string ConnectionString)
        {
            UserGroups = new UserGroupRepository(ConnectionString);
            Users = new UserIdentityRepository(ConnectionString);
        }
    }
}
