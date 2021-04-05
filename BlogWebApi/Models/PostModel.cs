using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BlogWebApi.Authentication;

namespace BlogWebApi.Models
{
    public class PostModel
    {

        public PostModel()
        {
            CategoryPosts = new HashSet<CategoryPostModel>();
        }
      
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UserId { get; set; }
        public string FilePath { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<CommentModel> Comments { get; set; }
        public virtual ICollection<CategoryPostModel> CategoryPosts { get; set; }
    }
}
