using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class FileCategoryRepository : BaseRepository<FileCategory>
    {
        public FileCategoryRepository(string connString) : base(connString)
        {
        }
    }
}