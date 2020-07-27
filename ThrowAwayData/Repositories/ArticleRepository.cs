using ThrowAwayData;

namespace ThrowAwayDataBackground
{
    public class ArticleRepository: BaseRepository<Article>
    {
        public ArticleRepository(string connectionString):base(connectionString) 
        { }
    }
}
