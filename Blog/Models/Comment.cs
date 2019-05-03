using System;

namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public string AuthorName { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastEditTime { get; set; }
        public State State { get; set; }
        public Post Post { get; set; }              
    }
}
