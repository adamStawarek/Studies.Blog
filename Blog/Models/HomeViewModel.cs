using System.Collections.Generic;

namespace Blog.Models
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Tag> Tags { get; set; }
    }
}