using System.Collections.Generic;

namespace Blog.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Post> Posts { get; set; }
        public List<TagViewModel> TagViewModels { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchWord { get; set; }
    }
}