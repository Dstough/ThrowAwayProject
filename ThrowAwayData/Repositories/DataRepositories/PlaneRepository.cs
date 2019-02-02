using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class PlaneRepository : BaseRepository<Plane>
    {
        public PlaneRepository(string _connectionString) : base(_connectionString) 
        { }
    }
}