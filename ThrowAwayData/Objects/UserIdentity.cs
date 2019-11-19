using ThrowAwayDataBackground;

namespace ThrowAwayData
{
    public class UserIdentity : BaseObject
    {
        public int UserGroupId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Passphrase { get; set; }
        public string VerificationCode { get; set; }
        public bool Authenticated { get; set; }
        public bool Banned { get; set; }
        public bool Dead { get; set; }

        public UserIdentity() : base()
        {
            UserGroupId = 0;
            Email = "";
            UserName = "";
            Passphrase = "";
            VerificationCode = "";
            Authenticated = false;
            Banned = false;
            Dead = false;
        }
    }
}