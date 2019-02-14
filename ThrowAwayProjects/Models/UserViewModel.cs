using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class UserViewModel
    {
        public int GroupId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PassPhrase { get; set; }

        public UserViewModel() : this(new UserIdentity())
        {            
        }

        public UserViewModel(UserIdentity user)
        {
            GroupId = user.GroupId;
            Email = user.Email;
            UserName = user.UserName;
            PassPhrase = user.PassPhrase;
        }
    }
}