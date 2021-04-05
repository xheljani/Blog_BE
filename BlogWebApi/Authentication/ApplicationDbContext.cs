using BlogWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApi.Authentication
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CategoryPostModel>().HasKey(map => new { map.CategoryId, map.PostId });


        //    builder.Entity<CategoryModel>()
        //       .HasData(
        //       new CategoryModel()
        //       {
        //           Id = 1,
        //           Name = "Category1",
        //           Description = "Category1"
        //       },
        //       new CategoryModel()
        //       {
        //           Id = 2,
        //           Name = "Category2",
        //           Description = "Category2"
        //       },
        //       new CategoryModel()
        //       {
        //           Id = 3,
        //           Name = "Category3",
        //           Description = "Category3"
        //       },
        //       new CategoryModel()
        //       {
        //           Id = 4,
        //           Name = "Category4",
        //           Description = "Category4"

        //       });
        }
        public DbSet<PostModel> PostModel { get; set; }
        public DbSet<CommentModel> CommentModel { get; set; }
        public DbSet<CategoryModel> CategoryModel { get; set; }
        public DbSet<CategoryPostModel> CategoryPostModel { get; set; }
    }
}
