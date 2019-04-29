using System.Collections.Generic;

namespace Blog.Models.ViewModels
{
    public class NavigationPanelViewModel
    {
        public List<TagViewModel> TagViewModels { get; set; }
        public string SearchWord { get; set; }
    }
}
