using System;
using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class Database
    {
        public UserGroupRepository UserGroups { get; private set; }
        public UserIdentityRepository Users { get; private set; }
        public TagRepository Tags { get; private set; }
        public ThreadRepository Threads { get; private set; }
        public PostRepository Posts { get; private set; }

        public Database() : this("")
        {
        }

        public Database(string ConnectionString)
        {
            UserGroups = new UserGroupRepository(ConnectionString);
            Users = new UserIdentityRepository(ConnectionString);
            Tags = new TagRepository(ConnectionString);
            Threads = new ThreadRepository(ConnectionString);
            Posts = new PostRepository(ConnectionString);
        }
    }
}
