using BlogWebApi.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApi.Authentication
{
    public class ApplicationUser : IdentityUser
    {

        public virtual ICollection<PostModel> Posts { get; set; }
        public virtual ICollection<CommentModel> Comments { get; set; }
    }
}
