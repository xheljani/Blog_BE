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
   // [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostModel>>> GetCategories()
        {

            var categories = await _context.CategoryModel.ToListAsync();

            return Ok(categories);

        }

    }
}
