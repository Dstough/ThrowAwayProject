using System;
using System.Collections.Generic;
using ThrowAwayData;
namespace ThrowAwayProjects.Models
{
    public class ThreadViewModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string CSS { get; set; }
        public DateTime PostDate { get; set; }
        public List<PostViewModel> Posts { get; set; }

        public ThreadViewModel(): this(new Thread())
        {
        }

        public ThreadViewModel( Thread thread)
        {
            Url = "/Forum/Thread/" + thread.Id;
            Title = thread.Title;
            Body = thread.Body;
            Author = null;
            CSS = null;
            PostDate = thread.CreatedOn;
            Posts = null;
        }
    }
}