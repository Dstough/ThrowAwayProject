﻿using System;
using System.Collections.Generic;
using ThrowAwayData;

namespace ThrowAwayProjects.Models
{
    public class ThreadViewModel
    {
        public string publicId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string Style { get; set; }
        public DateTime PostDate { get; set; }
        public bool Edited { get; set; }
        public List<PostViewModel> Posts { get; set; }

        public ThreadViewModel(): this(new Thread())
        {
        }

        public ThreadViewModel(Thread thread)
        {
            publicId = "0";
            Url = "/Forum/Thread/" + thread.PublicId;
            Title = thread.Title;
            Body = thread.Body;
            Author = null;
            Style = null;
            PostDate = thread.CreatedOn;
            Edited = false;
            Posts = new List<PostViewModel>();
        }
    }
}