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
using System.IO;
using System.Net;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace BlogWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/PostModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetPostModel(string searchString = "")
        {
            IList<PostModel> posts;
            if (string.IsNullOrEmpty(searchString))
            {
                 posts = await _context.PostModel
                    .Include(x => x.User)
                    .Include(x => x.CategoryPosts)
                    .OrderByDescending(x=>x.CreateTime)
                    .ToListAsync();
               
            }
            else
            {
                posts = await _context.PostModel.Include(x => x.CategoryPosts)
                     .Include(x => x.User)
                    .Where(x => x.Content.ToLower().Contains(searchString.ToLower()) ||
                    x.Title.ToLower().Contains(searchString.ToLower()))
                    .OrderByDescending(x => x.CreateTime)
                    .ToListAsync();

            }
            posts.ToList().ForEach(x => x.FilePath = GetBase64(x.FilePath));
            return Ok(posts);

        }

        [HttpGet("category/{id}")]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetPostModelByCategory(int id)
        {
            var postModel = await _context.PostModel
                .Include(x => x.CategoryPosts).Include(x => x.User)
                .Where(x => x.CategoryPosts.Any(c => c.CategoryId == id))
                .OrderByDescending(x => x.CreateTime)
                .ToListAsync();

            postModel.ToList().ForEach(x => x.FilePath = GetBase64(x.FilePath));
            return Ok(postModel);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetPostByUser(string id)
        {
            var postModel = await _context.PostModel
                .Include(x => x.CategoryPosts).Include(x => x.User)
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.CreateTime)
                .ToListAsync();

            postModel.ToList().ForEach(x => x.FilePath = GetBase64(x.FilePath));
            return Ok(postModel);
        }


        // GET: api/PostModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostModel>> GetPostModel(int id)
        {
            var postModel = await _context.PostModel
                .Include(x => x.CategoryPosts)
                .Include(x => x.User)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (postModel == null)
            {
                return NotFound();
            }
            postModel.Comments = postModel.Comments.OrderByDescending(x => x.CreateTime).ToList();
            postModel.FilePath = GetBase64(postModel.FilePath);

            return postModel;
        }

        // PUT: api/PostModels/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostModel(int id, PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return BadRequest();
            }


            var oldMappings = await _context.CategoryPostModel.Where(x => x.PostId == id).ToListAsync();
            if (oldMappings != null)
            {
                _context.CategoryPostModel.RemoveRange(oldMappings);
                _context.CategoryPostModel.AddRange(postModel.CategoryPosts);
            }

            _context.Entry(postModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostModelExists(id))
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

        // POST: api/PostModels
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PostModel>> PostPostModel(PostModel postModel)
        {
            var mappings = postModel.CategoryPosts;

            _context.PostModel.Add(postModel);

            mappings.ToList().ForEach(x => x.PostId = postModel.Id);

            _context.CategoryPostModel.AddRange(mappings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostModel", new { id = postModel.Id }, postModel);
        }

        // DELETE: api/PostModels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PostModel>> DeletePostModel(int id)
        {
            var postModel = await _context.PostModel.FindAsync(id);
            if (postModel == null)
            {
                return NotFound();
            }

            var oldMappings = await _context.CategoryPostModel.Where(x => x.PostId == id).ToListAsync();
            if (oldMappings != null)
            {
                _context.CategoryPostModel.RemoveRange(oldMappings);
            }
            _context.PostModel.Remove(postModel);
            await _context.SaveChangesAsync();

            return postModel;
        }

        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<ActionResult> CreatePostWithPhoto()
        {
            try
            {
                // save post
                Request.Form.TryGetValue("data", out StringValues data);
                if (StringValues.IsNullOrEmpty(data))
                {
                    return BadRequest();
                }

                PostModel post = JsonConvert.DeserializeObject<PostModel>(data);
                _context.PostModel.Add(post);
                if (await _context.SaveChangesAsync() > 0)
                {
                    //save file
                    var file = Request.Form.Files[0];
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Resources");

                    if (file.Length > 0)
                    {
                        var uniqueFileName = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                        var fullPath = Path.Combine(pathToSave, uniqueFileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        post.FilePath = uniqueFileName;
                        _context.Entry(post).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }

                }

                return CreatedAtAction("GetPostModel", new { id = post.Id }, post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }


        }




        [HttpPut("upload/{postId}"), DisableRequestSizeLimit]
        public async Task<ActionResult> UploadPostImage(int postId)
        {
            try
            {
                var file = Request.Form.Files[0];
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Resources");

                if (file.Length > 0)
                {
                    var postModel = await _context.PostModel.FindAsync(postId);
                    if (postModel == null)
                    {
                        return NotFound();
                    }

                    var uniqueFileName = DateTimeOffset.Now.ToUnixTimeSeconds().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    var fullPath = Path.Combine(pathToSave, uniqueFileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    postModel.FilePath = uniqueFileName;
                    _context.Entry(postModel).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }


        [HttpGet("image/{postId}"), ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetPostImage(int postId)
        {
            try
            {
                var postModel = await _context.PostModel.FindAsync(postId);
                if (postModel == null)
                {
                    return NotFound();
                }

                var fullPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Resources"), postModel.FilePath);

                byte[] imageBytes = System.IO.File.ReadAllBytes(fullPath);
                string base64String = Convert.ToBase64String(imageBytes);

                return Ok(base64String);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }


        private bool PostModelExists(int id)
        {
            return _context.PostModel.Any(e => e.Id == id);
        }

        private string GetBase64(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var fullPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Resources"), filePath);

                byte[] imageBytes = System.IO.File.ReadAllBytes(fullPath);
                string base64String = Convert.ToBase64String(imageBytes);


                return "data:image/jpeg;base64," + base64String;
            }
            return "";
        }
    }
}
