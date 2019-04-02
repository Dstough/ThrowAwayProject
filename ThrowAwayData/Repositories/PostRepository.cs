using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository(string connString) : base(connString)
        {
        }
    }
}