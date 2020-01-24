using System;
using System.Collections.Generic;
using ThrowAwayData;

namespace ThrowAwayProjects.Models
{
    public class ThreadViewModel
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string Style { get; set; }
        public DateTime PostDate { get; set; }
        public List<PostViewModel> Posts { get; set; }

        public ThreadViewModel(): this(new Thread())
        {
        }

        public ThreadViewModel(Thread thread)
        {
            Id = "0";
            Url = "/Forum/Thread/" + thread.PublicId;
            Title = thread.Title;
            Body = thread.Body;
            Author = null;
            Style = null;
            PostDate = thread.CreatedOn;
            Posts = new List<PostViewModel>();
        }
    }
}