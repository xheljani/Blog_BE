using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWebApi.Authentication;
using BlogWebApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CommentModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentModel>>> GetCommentModel()
        {
            return await _context.CommentModel.ToListAsync();
        }

        // GET: api/CommentModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentModel>> GetCommentModel(int id)
        {
            var commentModel = await _context.CommentModel.FindAsync(id);

            if (commentModel == null)
            {
                return NotFound();
            }

            return commentModel;
        }


        // GET: api/CommentModels/5
        [HttpGet("post/{id}")]
        public async Task<ActionResult<IEnumerable<CommentModel>>> GetCommentModelByPostId(int id)
        {
            var commentModels = await _context.CommentModel.Where(x => x.PostId == id).ToListAsync();

            return Ok(commentModels);

        }


        // PUT: api/CommentModels/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommentModel(int id, CommentModel commentModel)
        {
            if (id != commentModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(commentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentModelExists(id))
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

        // POST: api/CommentModels
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CommentModel>> PostCommentModel(CommentModel commentModel)
        {
            _context.CommentModel.Add(commentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommentModel", new { id = commentModel.Id }, commentModel);
        }

        // DELETE: api/CommentModels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CommentModel>> DeleteCommentModel(int id)
        {
            var commentModel = await _context.CommentModel.FindAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }

            _context.CommentModel.Remove(commentModel);
            await _context.SaveChangesAsync();

            return commentModel;
        }

        private bool CommentModelExists(int id)
        {
            return _context.CommentModel.Any(e => e.Id == id);
        }
    }
}
