using System;

namespace ThrowAwayProjects.Models
{
    public class PostViewModel
    {
        public int? Id { get; set; }
        public string Section { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string CSS { get; set; }
        public DateTime PostDate { get; set; }
    }
}
