using System;

namespace ThrowAwayData
{
    public partial class Database
    {
        public void CloseWeekOldThreads()
        {
            try
            {
                var dbThreads = Threads
                    .Where(new { Closed = 0 })
                    .Where(new { OperatorSymbol = "<=", CreatedOn = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd hh-mm-ss") })
                    .Find();

                foreach (var item in dbThreads)
                {
                    item.Closed = true;
                    Threads.Edit(item);
                }
            }
            catch (Exception)
            {
            }
        }

        public void AllowWeekOldUsersToPost()
        {
            try
            {
            }
            catch(Exception)
            {
            }
        }
    }
}
