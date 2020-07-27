using System;
using System.Linq;

namespace ThrowAwayData
{
    public partial class Database
    {
        public void CloseWeekOldThreads()
        {
            try
            {
                Threads
                    .Where(new { Closed = 0 })
                    .Where(new { OperatorSymbol = "<=", CreatedOn = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd hh-mm-ss") })
                    .Find().ToList().ForEach(thread =>
                    {
                        thread.Closed = true;
                        Threads.Edit(thread);
                    });
            }
            catch (Exception)
            {
            }
        }

        public void AllowWeekOldUsersToPost()
        {
            try
            {
                var chummerGroup = UserGroups.Where(new { Name = "Chummer" }).Find().FirstOrDefault();
                var userGroup = UserGroups.Where(new { Name = "User" }).Find().FirstOrDefault();

                if (chummerGroup == null || userGroup == null)
                    return;

                Users
                    .Where(new { UserGroupId = chummerGroup.Id })
                    .Where(new { OperatorSymbol = "<=", CreatedOn = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd hh-mm-ss") })
                    .Find().ToList().ForEach(user => 
                    {
                        user.UserGroupId = userGroup.Id ?? 0;
                        Users.Edit(user);
                    });
            }
            catch (Exception)
            {
            }
        }
    }
}
