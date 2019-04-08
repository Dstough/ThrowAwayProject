using System;
namespace ThrowAwayProjects.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public DateTime DateSessionStarted { get; set; }
        public int AccountAge { get; set; }

        public HomeViewModel()
        {
            DateSessionStarted = DateTime.Now;
            AccountAge = 0;
        }
    }
}