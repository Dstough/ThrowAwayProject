using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string PassPhrase { get; set; }
        public string PassPhraseConfirm { get; set; }
        public string Email { get; set; }
        public string TargetAction { get; set; }
        public string TargetController { get; set; }
        public string ErrorMessage { get; set; }

        public UserViewModel() : this(new UserIdentity())
        {
        }

        public UserViewModel(UserIdentity user)
        {
            UserName = user.UserName;
            PassPhrase = user.PassPhrase;
            Email = user.Email;
            TargetAction = "Index";
            TargetController = "Home";
        }
    }
}