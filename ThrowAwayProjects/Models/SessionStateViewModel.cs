using System;
namespace ThrowAwayProjects.Models
{
    public class SessionStateViewModel
    {
        public DateTime DateSessionStarted { get; set; }
        public DateTime Now { get; set; }

        public SessionStateViewModel()
        {
            DateSessionStarted = DateTime.Now;
            Now = DateTime.Now;
        }
    }
}