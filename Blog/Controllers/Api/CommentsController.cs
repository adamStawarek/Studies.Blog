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
        [HttpGet("count")]
        public int GetCommentsCount()
        {
            return _context.Comments.Count();
        }

        // GET: api/Comments/page/2
        [HttpGet("page/{pageNo}")]
        public IEnumerable<dynamic> GetComments(int pageNo)
        {
            return _context.Comments
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

        // GET: api/Comments
        [HttpGet("post/{id}")]
        public IEnumerable<dynamic> GetPostComments([FromRoute]int id)
        {
            return _context.Comments.Include(c=>c.User).Where(c=>c.PostId==id)
                .Select(c=>new {Content=c.Content,CreationTime=c.CreationTime,Id=c.Id
                    ,LastEditTime=c.LastEditTime,PostId=c.PostId,State=c.State,Author=c.User.Name});
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment([FromRoute] int id, [FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}