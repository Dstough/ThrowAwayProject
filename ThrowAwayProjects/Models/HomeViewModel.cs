using System;
namespace ThrowAwayProjects.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public string AccountStatus { get; set; }
        public string Privilege { get; set; }
        public DateTime DateSessionStarted { get; set; }

        public HomeViewModel()
        {
            AccountStatus = "Not Logged In";
            Privilege = "Not Logged In";
            DateSessionStarted = DateTime.Now;
        }
    }
}