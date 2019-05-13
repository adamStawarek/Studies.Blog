using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Blog.Models
{
    public static class BlogInitializer
    {
        private static readonly Random Rnd;

        static BlogInitializer()
        {
            Rnd = new Random();
        }

        public static void Initialize(BlogContext context)
        {            
            context.Database.EnsureCreated();
            if(context.Posts.Any()) return;

            IList<Post> posts = GetPosts(10);
            IList<Tag> tags = GetTags();

            context.Tags.AddRange(tags);
            context.SaveChanges();

            context.Posts.AddRange(posts);
            context.SaveChanges();

          
            foreach (var post in posts)
            {
                var randomTags = tags.Where(t => t.Id < Rnd.Next(tags.Count));
                foreach (var randomTag in randomTags)
                {
                    context.PostTags.Add(new PostTag()
                    {
                        Post = post,
                        Tag = randomTag
                    });
                    context.SaveChanges();
                }               
            }
        }

       

        private static List<Post> GetPosts(int postCount)
        {
            var tags = GetTags();
            var posts = new List<Post>();
            for (var i = 0; i < postCount; i++)
            {
                posts.Add(new Post()
                {
                    UserId = System.Security.Claims.ClaimsPrincipal.Current.Claims.First(c => c.Properties.Values.Contains("sub"))?.Value,
                    Title = $"Post #{i}",
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut" +
                              " labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi " +
                              "ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum " +
                              "dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia" +
                              " deserunt mollit anim id est laborum.",
                    CreationTime = DateTime.Today.AddDays(i * (-1)),
                    LastEditTime = DateTime.Today,
                    Stars = Rnd.Next(5),
                    Image = $"https://picsum.photos/200/?image={1060+i}"
                });
            }

            return posts;
        }

        private static List<Tag> GetTags()
        {
            return new List<Tag>{
            new Tag(){Name="Tag1"},
            new Tag(){Name="Tag2"},
            new Tag(){Name="Tag3"},
            new Tag(){Name="Tag4"},
            new Tag(){Name="Tag5"},
            new Tag(){Name="Tag6"},
            new Tag(){Name="Tag7"},
            new Tag(){Name="Tag8"},
            new Tag(){Name="Tag9"},
            new Tag(){Name="Tag10"}
        };
        }
    }
}
