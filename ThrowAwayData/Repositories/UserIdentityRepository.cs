using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class UserIdentityRepository : BaseRepository<UserIdentity>
    {
        public UserIdentityRepository(string connString) : base(connString)
        {
        }
    }
}