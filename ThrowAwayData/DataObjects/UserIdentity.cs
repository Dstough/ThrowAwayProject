using ThrowAwayDataBackground;
namespace ThrowAwayData
{
    public class UserIdentity : BaseObject
    {
        public int GroupId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PassPhrase { get; set; }
    }
}