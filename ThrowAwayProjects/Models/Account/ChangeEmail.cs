using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class ChangeEmailViewModel : BaseViewModel
    {
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string Passphrase { get; set; }
        public string AuthenticationCode { get; set; }

        public ChangeEmailViewModel() : this(new UserIdentity())
        {
        }

        public ChangeEmailViewModel(UserIdentity user)
        {
            OldEmail = user.Email;
        }
    }
}