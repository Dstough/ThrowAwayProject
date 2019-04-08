namespace ThrowAwayProjects.Models
{
    public class VerificationViewModel : BaseViewModel
    {
        public string dbGuid { get; set; }
        public string inputGuid { get; set; }
        public VerificationViewModel()
        {
            dbGuid = null;
            inputGuid = null;
        }
    }
}