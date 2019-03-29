using System;
using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class UnitOfWork
    {
        public UserGroupRepository UserGroups { get; private set; }
        public UserIdentityRepository Users { get; private set; }

        public UnitOfWork() : this("")
        {
        }

        public UnitOfWork(string ConnectionString)
        {
            UserGroups = new UserGroupRepository(ConnectionString);
            Users = new UserIdentityRepository(ConnectionString);
        }
    }
}
