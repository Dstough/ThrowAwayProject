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
                    .Where(new { Closed = false })
                    .Where(new { OperatorSymbol = "<=", CreatedOn = DateTime.Now.AddDays(-7) })
                    .Find();

                foreach(var item in dbThreads)
                {
                    item.Closed = true;
                    Threads.Edit(item);
                }
            }
            catch (Exception)
            {
                //Do Nothing
            }
        }
    }
}
