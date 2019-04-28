using System.Collections.Generic;

namespace Blog.Models
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Tag> Tags { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchWord { get; set; }
    }
}