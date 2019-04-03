using System;
using System.Collections.Generic;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastEditTime { get; set; }
        public List<string> Tags { get; set; }
        public string Author { get; set; }
    }
}