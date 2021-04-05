using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BlogWebApi.Authentication;

namespace BlogWebApi.Models
{
    public class CategoryPostModel
    {

        public int CategoryId { get; set; }
        public int PostId { get; set; }

        public virtual CategoryModel Category { get; set; }
        public virtual PostModel Post { get; set; }

    }
}
