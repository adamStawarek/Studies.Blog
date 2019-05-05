using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blog.Helpers;
using Blog.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private const double PostsPerPage = 4.0;
        private const int MaxPopularTagsCount = 10;
        private readonly BlogContext _context;

        public HomeController(BlogContext context)
        {
            this._context = context;
            CreateLogger();
        }

        private void CreateLogger()
        {
            //TODO move connection string to appsettings.json
            var logDB = @"Server=DESKTOP-HC7DAS3\ADAMSQL;Database=Blog;Trusted_Connection=True;";
            var logTable = "Logs";
            var sink = new MSSqlServerSink(logDB, logTable, 1, TimeSpan.FromSeconds(1), null, true);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .WriteTo.Sink(sink)
                .CreateLogger();
        }

        public IActionResult Index(int currentPage=1, string searchWord="",List<int> tagIds=null)
        {                     
            List<Post> posts = null;
            int totalPages = 0;
            List<TagViewModel> tagVms = null;

            try
            {
                if (tagIds!=null&&tagIds.Count>0)
                {
                    posts = _context.Posts
                                .Where(p => p.Content.ToLower().Contains(searchWord.ToLower()))
                                .Include(p => p.PostTags).ThenInclude(p => p.Tag)
                                .Where(p => p.PostTags.Any(t => tagIds.Contains(t.TagId)))
                                .OrderByDescending(d => d.Id)
                                .Batch(4)
                                .ElementAt(currentPage - 1)
                                .ToList();

                   totalPages = (int)Math.Ceiling(_context.Posts.Count(p => 
                        p.Content.ToLower().Contains(searchWord.ToLower())
                        && p.PostTags.Any(t=>tagIds.Contains(t.TagId))) / PostsPerPage);

                    tagVms = GetPopularTags(MaxPopularTagsCount).Select(t => new TagViewModel()
                        {Tag = t, IsSelected = tagIds.Contains(t.Id)}).ToList();
                }
                else
                {
                    var word = searchWord.ToLower();
                    posts = _context.Posts
                        .Where(p => p.Content==null || p.Content.ToLower().Contains(word))
                        .Include(p => p.PostTags).ThenInclude(p => p.Tag)
                        .OrderByDescending(d => d.Id)
                        .Batch(4)
                        .ElementAt(currentPage - 1)
                        .ToList();

                    totalPages = (int)Math.Ceiling(_context.Posts.Count(p =>
                        p.Content == null || p.Content.ToLower().Contains(word)) / PostsPerPage);

                    tagVms = GetPopularTags(MaxPopularTagsCount).Select(t => new TagViewModel() {Tag = t}).ToList();
                }             
            }
            catch 
            {
                Log.Warning($"No post contains '{searchWord}'");
            }

            searchWord = searchWord ?? "";
            var vm = new HomeViewModel()
            {
                SearchWord = searchWord,
                Posts = posts??new List<Post>(),
                TagViewModels =tagVms,
                CurrentPage = currentPage,
                TotalPages =totalPages
            };
            return View(vm);
        }

        public ActionResult SearchByTags(List<int> ids)
        {
            try
            {
                return Json(new { ok = true, newurl = Url.Action("Index","Home",new {tagIds=ids}) });
            }
            catch (Exception ex)
            {
                Log.Write(LogEventLevel.Error,ex,"Cannot redirect with ajax");
                return Json(new { ok = false, message = ex.Message });
            }
        }

        private List<Tag> GetPopularTags(int n)
        {
            var tags = _context.PostTags.GroupBy(p => p.Tag)
                .Select(p => new {tag = p.Key, count = p.Count()})
                .OrderByDescending(p => p.count)
                .Select(p=>p.tag)
                .Take(n).ToList();
            return tags;
        }

        public IActionResult Details(int id)
        {
            var post = _context.Posts.Include(p=>p.Comments).FirstOrDefault(p => p.Id == id);
            return View(post);
        }  

        [HttpPost]
        [Authorize(Roles = "Admins")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return new BadRequestResult();

            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            _context.Posts.Remove(post ?? throw new InvalidOperationException());
            _context.SaveChanges();
            string prevUrl = Request.Headers["Referer"].ToString();
            return Redirect(prevUrl);
        }

        [Authorize(Roles = "Admins")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return new BadRequestResult();

            var post = _context.Posts.Include(p=>p.PostTags).ThenInclude(p=>p.Tag).FirstOrDefault(p => p.Id == id);
            if (post == null)
                return new BadRequestResult();

            var model=new CreatePostViewModel()
            {
                Post = post,
                Tags=string.Join(',',post.PostTags.Select(p=>p.Tag.Name).ToList())
            };
            return View(model);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreatePostViewModel model)
        {
            if (!ModelState.IsValid) return View();

            string imageUrl = await UploadImageAsync(model);

            var post = _context.Posts.First(p => p.Id == model.Post.Id);
            post.Content = model.Post.Content;
            post.Title = model.Post.Title;
            post.Image = imageUrl ?? model.Post.Image;
            _context.SaveChanges();

            UpdateTags(model, post);

            return RedirectToAction("Details", new { id = model.Post.Id });
        }

        [Authorize(Roles = "Admins")]
        public IActionResult Create()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
           
            if (!ModelState.IsValid)
            {
                return View();
            }

            string imageUrl = await UploadImageAsync(model);

            var post = model.Post;
            var newPost = new Post()
            {
                Title = post.Title,
                Content = post.Content,
                PostTags = post.PostTags,
                Author = HttpContext.User.Identity.Name,
                CreationTime = DateTime.Today,
                LastEditTime = DateTime.Today,
                Image = imageUrl ?? model.Post.Image,
                Stars = 0
            };
            _context.Posts.Add(newPost);
            _context.SaveChanges();

            UpdateTags(model, newPost);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> AddComment(string content,int? postId)
        {
            if (postId == null) return new BadRequestResult();
            var comment=new Comment()
            {
                AuthorId = 1,AuthorName = User.Identity.Name,Content = content,CreationTime = DateTime.Now,LastEditTime = DateTime.Now,
                PostId = (int)postId,State = State.WaitingForApproval
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details","Home",new {id=postId});
        }

        private static async Task<string> UploadImageAsync(CreatePostViewModel model)
        {
            string imageUrl = null;
            if (model.FormFile != null
                && !model.Post.Image.StartsWith("https")
                && !model.Post.Image.StartsWith("http"))
            {
                var fileId = await DriveApi.UploadFile(model.FormFile);
                imageUrl = $"https://drive.google.com/uc?export=view&id={fileId}";
            }

            return imageUrl;
        }

        private void UpdateTags(CreatePostViewModel model, Post post)
        {
            var tags = model.Tags.Split(',').ToList();
            foreach (var tag in tags)
            {
                if (!_context.Tags.Any(t => string.Equals(t.Name, tag, StringComparison.CurrentCultureIgnoreCase)))
                {
                    _context.Tags.Add(new Tag()
                    {
                        Name = tag
                    });
                    _context.SaveChanges();
                }
            }

            tags = tags.Select(t => t.ToLower()).ToList();
            var tagsFromDb = _context.Tags.Where(t => tags.Contains(t.Name.ToLower())).ToList();

            var oldPostTags = _context.PostTags.Where(p => p.PostId == post.Id).ToList();
            _context.PostTags.RemoveRange(oldPostTags);
            _context.SaveChanges();

            foreach (var tag in tagsFromDb)
            {
                _context.PostTags.Add(new PostTag()
                {
                    Tag = tag,
                    Post = post
                });
                _context.SaveChanges();
            }
        }

        #region error handling

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion       
    }
}