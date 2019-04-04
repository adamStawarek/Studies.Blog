using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Random Rnd=new Random();
        private static readonly List<Tag> Tags=new List<Tag>{
            new Tag(){Id=1,Name="Tag1"},
            new Tag(){Id=2,Name="Tag2"},
            new Tag(){Id=3,Name="Tag3"},
            new Tag(){Id=4,Name="Tag4"},
            new Tag(){Id=5,Name="Tag5"},
            new Tag(){Id=6,Name="Tag6"},
            new Tag(){Id=7,Name="Tag7"},
            new Tag(){Id=8,Name="Tag8"},
            new Tag(){Id=9,Name="Tag9"},
            new Tag(){Id=10,Name="Tag10"}
        };
        private static readonly List<Post> Posts=GetPosts();
            
        private static List<Post> GetPosts()
        {
            var postCount = 10;
            var posts=new List<Post>();
            for (int i = 0; i < postCount; i++)
            {
                posts.Add(new Post()
                {
                    Id = i,
                    Author = "adam stawarek",
                    Title = $"Post #{i}",
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut" +
                              " labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " +
                              "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum " +
                              "dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia" +
                              " deserunt mollit anim id est laborum.",
                    CreationTime = DateTime.Today.AddDays(i*(-1)),
                    LastEditTime = DateTime.Today,
                    Tags = Tags.Where((t=>t.Id<Rnd.Next(Tags.Count))).ToList(),
                    Stars = Rnd.Next(5),
                    Image = $"https://picsum.photos/200/?image={1060+i}"
                });
            }

            return posts;
        }
        
        public IActionResult Index()
        {
            var vm=new HomeViewModel()
            {
                Posts = Posts,
                Tags = Tags
            };
            return View(vm);
        }

        public IActionResult Details(int id)
        {
            var post = Posts.FirstOrDefault(p => p.Id == id);
            return View(post);
        }        
       
        [HttpPost]
        public IActionResult Delete(int? id)
        {            
            if(id==null)
                return new BadRequestResult();
            Posts.RemoveAll(p=>p.Id==id);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return new BadRequestResult();
            var post =  Posts.Find(j => j.Id == id);
            if(post==null)
                return new BadRequestResult();
            return View(post);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public IActionResult Edit(Post post)
        {
            if (!ModelState.IsValid) return View();
            var _post = Posts.Find(j => j.Id == post.Id);
             _post.Content = post.Content;
             _post.Title = post.Title;
            return RedirectToAction("Details", new { id = post.Id });
        }
        
        public IActionResult Create()
        {          
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public IActionResult Create(Post model)
        {
            if (!ModelState.IsValid)
            {               
                return View();
            }

            var id = Posts.Max(j => j.Id)+1;
            Posts.Add(new Post()
                {
                    Id = id,
                    Title = model.Title,
                    Content = model.Content,
                    Tags = model.Tags,
                    Author="adam stawarek",
                    CreationTime = DateTime.Today,
                    LastEditTime = DateTime.Today,
                    Image = $"https://picsum.photos/200/?image={1060+id}",
                    Stars = 0
                }
            );
            return RedirectToAction("Index");
        }      
        
        #region error handling

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        #endregion       
    }
}