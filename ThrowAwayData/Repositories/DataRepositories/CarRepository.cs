using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class CarRepository : BaseRepository<Car>
    {
        public CarRepository(string _connectionString) : base(_connectionString) 
        { }
    }
}