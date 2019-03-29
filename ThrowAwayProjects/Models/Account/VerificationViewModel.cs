namespace ThrowAwayProjects.Models
{
    public class VerificationViewModel
    {
        public int? UserId { get; set; }
        public string dbGuid { get; set; }
        public string inputGuid { get; set; }
        public VerificationViewModel()
        {
            UserId = null;
            dbGuid = null;
            inputGuid = null;
        }
    }
}