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
            Files = new FileRepository();
            FileCategories = new FileCategoryRepository();
            UserGroups = new UserGroupRepository();
            Users = new UserIdentityRepository();
        }
    }
}
