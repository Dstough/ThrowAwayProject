using System;
using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class UnitOfWork
    {
        FileRepository Files;
        FileCategoryRepository FileCategories;
        UserGroupRepository UserGroups;
        UserIdentityRepository Users;

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
