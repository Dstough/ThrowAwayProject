using ThrowAwayData;

namespace ThrowAwayDataBackground
{
    public class CommentRepository: BaseRepository<Comment>
    {
        public CommentRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
