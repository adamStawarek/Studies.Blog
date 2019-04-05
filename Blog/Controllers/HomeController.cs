﻿using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using static System.String;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogContext _context;

        public HomeController(BlogContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            var vm = new HomeViewModel()
            {
                Posts = _context.Posts.Include(p => p.PostTags).ThenInclude(p => p.Tag).OrderByDescending(d=>d.Id).ToList(),
                Tags = _context.Tags.ToList()
            };
            return View(vm);
        }

        public IActionResult Details(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            return View(post);
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return new BadRequestResult();

            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            _context.Posts.Remove(post ?? throw new InvalidOperationException());
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return new BadRequestResult();

            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
                return new BadRequestResult();
            return View(post);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public IActionResult Edit(Post post)
        {
            if (!ModelState.IsValid) return View();

            var _post = _context.Posts.FirstOrDefault(p => p.Id == post.Id);
            _post.Content = post.Content;
            _post.Title = post.Title;
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = post.Id });
        }

        public IActionResult Create()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.ValidateAntiForgeryToken]
        public IActionResult Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var maxId = _context.Posts.Max(p => p.Id);
            var post = model.Post;
            var newPost = new Post()
            {
                Title = post.Title,
                Content = post.Content,
                PostTags = post.PostTags,
                Author = "adam stawarek",
                CreationTime = DateTime.Today,
                LastEditTime = DateTime.Today,
                Image = $"https://picsum.photos/200/?image={maxId%1085}",
                Stars = 0
            };
            _context.Posts.Add(newPost);
            _context.SaveChanges();

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

            foreach (var tag in tagsFromDb)
            {
                _context.PostTags.Add(new PostTag()
                {
                    Tag = tag,
                    Post = newPost
                });
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
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