using ThrowAwayData;

namespace ThrowAwayDataBackground
{
    public class ThreadRepository : BaseRepository<Thread>
    {
        public ThreadRepository(string connString) : base(connString)
        {
        }
    }
}
