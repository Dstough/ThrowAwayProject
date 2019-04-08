using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class AccountViewModel : BaseViewModel
    {
        public string UserName { get; set; }
        public string Passphrase { get; set; }
        public string PassphraseConfirm { get; set; }
        public string Email { get; set; }
        public string TargetAction { get; set; }
        public string TargetController { get; set; }

        public AccountViewModel() : this(new UserIdentity())
        {
        }

        public AccountViewModel(UserIdentity user)
        {
            UserName = user.UserName;
            Passphrase = user.Passphrase;
            Email = user.Email;
            TargetAction = "Index";
            TargetController = "Home";
        }
    }
}