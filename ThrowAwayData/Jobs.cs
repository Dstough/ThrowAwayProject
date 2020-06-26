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
                var dbThreads = Threads
                    .Where(new { Closed = 0 })
                    .Find()
                    .Where(item => item.CreatedOn <= DateTime.Now.AddDays(-7));

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
    }
}
