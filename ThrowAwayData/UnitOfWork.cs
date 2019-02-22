using System;
using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class UnitOfWork
    {
        public FileRepository Files { get; private set; }
        public FileCategoryRepository FileCategories { get; private set; }
        public UserGroupRepository UserGroups { get; private set; }
        public UserIdentityRepository Users { get; private set; }

        public UnitOfWork() : this("")
        {
        }

        public UnitOfWork(string ConnectionString)
        {
            Files = new FileRepository(ConnectionString);
            FileCategories = new FileCategoryRepository(ConnectionString);
            UserGroups = new UserGroupRepository(ConnectionString);
            Users = new UserIdentityRepository(ConnectionString);
        }
    }
}
