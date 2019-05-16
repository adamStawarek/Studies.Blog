using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admins")]
    public class CommentsController : Controller
    {
        private readonly BlogContext _context;

        public CommentsController(BlogContext context)
        {
            _context = context;
        }

        // GET: Comments
        public IActionResult Index()
        {
            return View();
        }
      
    }
}
