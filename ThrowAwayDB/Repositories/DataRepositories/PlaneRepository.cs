using ThrowAwayDb;
namespace ThrowAwayDbBackground
{
    public class PlaneRepository : BaseRepository<Plane>
    {
        public PlaneRepository(string _connectionString) : base(_connectionString) 
        { }
    }
}