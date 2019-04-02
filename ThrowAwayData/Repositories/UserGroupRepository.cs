using ThrowAwayData;
namespace ThrowAwayDataBackground
{
    public class UserGroupRepository : BaseRepository<UserGroup>
    {
        public UserGroupRepository(string connString) : base(connString)
        {
        }
    }
}