using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string PassPhrase { get; set; }
        public string TargetUrl { get; set; }

        public UserViewModel() : this(new UserIdentity())
        {
        }

        public UserViewModel(UserIdentity user)
        {
            UserName = user.UserName;
            PassPhrase = user.PassPhrase;
            TargetUrl = "";
        }
    }
}