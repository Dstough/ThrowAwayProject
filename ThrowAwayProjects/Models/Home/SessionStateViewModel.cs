using System;
namespace ThrowAwayProjects.Models
{
    public class SessionStateViewModel : BaseViewModel
    {
        public DateTime DateSessionStarted { get; set; }
        public DateTime Now { get; set; }
        public int AccountAge {get; set;}

        public SessionStateViewModel()
        {
            DateSessionStarted = DateTime.Now;
            Now = DateTime.Now;
            AccountAge = 0;
        }
    }
}