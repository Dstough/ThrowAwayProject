using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class TagRepository : BaseRepository<Tag>
    {
        public TagRepository(string connString) : base(connString)
        {
        }
    }
}
