using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BlogWebApi.Authentication;

namespace BlogWebApi.Models
{
    public class CategoryModel
    {

        public CategoryModel()
        {
            CategoryPosts = new HashSet<CategoryPostModel>();
        }
      
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

       public virtual ICollection<CategoryPostModel> CategoryPosts { get; set; }
    }
}
