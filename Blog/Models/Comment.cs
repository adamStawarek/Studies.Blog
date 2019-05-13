using System;

namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastEditTime { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public State State { get; set; }
        public Post Post { get; set; }              
    }
}
