namespace ThrowAwayProjects.Models
{
    public class UpdateViewModel : BaseViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NewPassphrase { get; set; }
        public string ConfirmPassphrase { get; set; }
    }
}