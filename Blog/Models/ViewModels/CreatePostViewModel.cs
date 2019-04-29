using Microsoft.AspNetCore.Http;

namespace Blog.Models.ViewModels
{
    public class CreatePostViewModel
    {
        public Post Post { get; set; }
        public string Tags { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
