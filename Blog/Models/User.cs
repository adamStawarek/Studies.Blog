using System.Collections.Generic;

namespace Blog.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }
}