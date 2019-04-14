
using System;
using System.Collections.Generic;
using Blog.Controllers;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Blog.Tests.Integration
{

    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void Index_Returns_IActionResult_With_HomeViewModel_With_2_Posts()
        {
            var builder = new DbContextOptionsBuilder<BlogContext>();
            builder.UseInMemoryDatabase();
            var options = builder.Options;

            using (var context = new BlogContext(options))
            {               
                context.Posts.Add(new Post());
                context.Posts.Add(new Post());              
                context.SaveChanges();

                var controller = new HomeController(context);
                var result = controller.Index();
                var viewResult = result as ViewResult;

                var model = viewResult?.ViewData.Model as HomeViewModel;

                Assert.That(result, Is.InstanceOf<IActionResult>());
                Assert.IsAssignableFrom<HomeViewModel>(viewResult?.ViewData.Model);
                Assert.AreEqual(2, model?.Posts.Count);
            }
        }
    }
}
