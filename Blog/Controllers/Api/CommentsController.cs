using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using MoreLinq;

namespace Blog.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly BlogContext _context;
        private const int batchSize = 5; //number of comments on one page

        public CommentsController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/Comments/count
        [HttpGet("count/{state}")]
        public int GetCommentsCount(int state)
        {
            return _context.Comments.Count(c=>c.State==(State)state);
        }

        // GET: api/Comments/page/2
        [HttpGet("page/{state}/{pageNo}")]
        public IEnumerable<dynamic> GetComments(int state,int pageNo)
        {
            return _context.Comments
                .Where(c=>c.State==(State)state)
                .Include(c=>c.User)
                .Include(p=>p.Post)
                .Batch(batchSize)
                .ElementAt(pageNo-1)
                .Select(c => new {
                    Content = c.Content,
                    CreationTime = c.CreationTime,
                    Id = c.Id,
                    LastEditTime = c.LastEditTime,
                    PostId = c.PostId,
                    State = c.State,
                    Author = c.User.Name,
                    PostTitle=c.Post.Title
                }); 
        }

        // GET: api/Comments/post/122
        [HttpGet("post/{id}")]
        public IEnumerable<dynamic> GetPostComments([FromRoute]int id)
        {
            return _context.Comments.Include(c=>c.User).Where(c=>c.PostId==id&&c.State==State.Approved)
                .Select(c=>new {Content=c.Content,CreationTime=c.CreationTime,Id=c.Id
                    ,LastEditTime=c.LastEditTime,PostId=c.PostId,State=c.State,Author=c.User.Name});
        }
     
        // DELETE: api/Comments/5
        [HttpDelete("reject/{id}")]
        public IActionResult RejectComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.State = State.Rejected;
            _context.Comments.Update(comment);
            _context.SaveChanges();

            return Ok(comment);
        }

        // DELETE: api/Comments/5
        [HttpPut("approve/{id}")]
        public IActionResult ApproveComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.State = State.Approved;
            _context.Comments.Update(comment);
            _context.SaveChanges();

            return Ok(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}