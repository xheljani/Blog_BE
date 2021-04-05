using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using BlogWebApi.Authentication;
using System.Linq;
using System.Threading.Tasks;

namespace BlogWebApi.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public virtual ApplicationUser User{ get; set; }
        public virtual PostModel Post { get; set; }
    }
}
