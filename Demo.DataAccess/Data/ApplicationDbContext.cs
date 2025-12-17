using Demo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Demo.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Fiction", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Drama", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
            new Product 
                { 
                    Id = 2,
                    Title = "Fortune of Time",
                    Author = "Billy Spark",
                    ISBN = "SWD999901",
                    Description = "Lorem ipsum",
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    TotallyNotAnID = 666,
                    CategoryId = 3,
                    ImageUrl = "",
                }
            );
        }
    }
}
