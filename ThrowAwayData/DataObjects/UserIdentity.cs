using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class UserIdentity : BaseObject
    {
        public int GroupId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PassPhrase { get; set; }
        public bool Authenticated { get; set; }
        public bool CanPost { get; set; }
        public bool Banned { get; set; }
        public bool Dead { get; set; }
        public UserIdentity() : base()
        {
            GroupId = 0;
            Email = "";
            UserName = "";
            PassPhrase = "";
            Authenticated = false;
            CanPost = false;
            Banned = false;
            Dead = false;
        }
    }
}