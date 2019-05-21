using System;
using System.Collections.Generic;

namespace ThrowAwayProjects.Models
{
    public class ThreadViewModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string CSS { get; set; }
        public DateTime PostDate { get; set; }
        public List<PostViewModel> Posts { get; set; }
    }
}