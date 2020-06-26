using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public partial class Database
    {
        public TagRepository Tags { get; private set; }
        public PostRepository Posts { get; private set; }
        public ThreadRepository Threads { get; private set; }
        public UserGroupRepository UserGroups { get; private set; }
        public UserIdentityRepository Users { get; private set; }

        public Database() : this("")
        { }

        public Database(string ConnectionString)
        {
            Tags = new TagRepository(ConnectionString);
            Posts = new PostRepository(ConnectionString);
            Users = new UserIdentityRepository(ConnectionString);
            Threads = new ThreadRepository(ConnectionString);
            UserGroups = new UserGroupRepository(ConnectionString);
        }
    }
}