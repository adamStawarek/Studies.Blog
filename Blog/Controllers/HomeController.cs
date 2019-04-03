﻿using System;
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
        public IActionResult Index()
        {
            var posts = GetPosts();
            return View(posts);
        }

        private List<Post> GetPosts()
        {
            var postCount = 10;
            var posts=new List<Post>();
            for (int i = 0; i < postCount; i++)
            {
                posts.Add(new Post()
                {
                    Id = 1,
                    Author = "adam stawarek",
                    Title = $"Post #{i}",
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut" +
                              " labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " +
                              "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum " +
                              "dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia" +
                              " deserunt mollit anim id est laborum.",
                    CreationTime = DateTime.Today.AddDays(i*(-1)),
                    LastEditTime = DateTime.Today,
                    Tags = new List<string>{"tag"}
                });
            }

            return posts;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}