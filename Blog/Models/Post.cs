using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastEditTime { get; set; }       
        public int Stars { get; set; }
        public string Image { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        [DisplayName("TagViewModels")]
        public List<PostTag> PostTags { get; set; } = new List<PostTag>();

        public List<Comment> Comments { get; set; }=new List<Comment>();
    }
}