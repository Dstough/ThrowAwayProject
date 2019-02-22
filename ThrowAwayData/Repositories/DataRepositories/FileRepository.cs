using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class FileRepository : BaseRepository<FileObject>
    {
        public FileRepository(string connString) : base(connString)
        {
        }
    }
}