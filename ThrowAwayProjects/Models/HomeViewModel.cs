using System.Collections.Generic;
namespace ThrowAwayProjects.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public List<ThreadViewModel> UserThreads { get; set; }

        public HomeViewModel()
        {
            UserThreads = new List<ThreadViewModel>();
        }
    }
}