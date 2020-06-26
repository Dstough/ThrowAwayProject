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

                Console.WriteLine($"Found {dbThreads.Count()} threads to close");

                foreach (var item in dbThreads)
                {
                    item.Closed = true;
                    Threads.Edit(item);

                    Console.WriteLine($"Closed Thread {item.PublicId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString().Replace(" at ", "\n"));
            }
        }
    }
}
