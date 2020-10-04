using System.Collections.Generic;
namespace ThrowAwayProjects.Models
{
    public class HomeViewModel : BaseViewModel
    {
        public List<ThreadViewModel> UserThreads { get; set; }
        public List<string> NewsArticles { get; set; }
        public List<string> JobArticles { get; set; }
        public List<string> RunArticles { get; set; }

        public HomeViewModel()
        {
            UserThreads = new List<ThreadViewModel>();
            NewsArticles = new List<string>();
            JobArticles = new List<string>();
            RunArticles = new List<string>();
        }
    }
}