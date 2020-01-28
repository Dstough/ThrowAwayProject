using System;

namespace ThrowAwayProjects.Models
{
    public class PostViewModel
    {
        public int? Id { get; set; }
        public bool Edited { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string Style { get; set; }
        public DateTime PostDate { get; set; }

        public PostViewModel()
        {
            Id = null;
            Edited = false;
            Body = "";
            Author = "";
            Style = "";
            PostDate = DateTime.Now;
        }
    }
}
