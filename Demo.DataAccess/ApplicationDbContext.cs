using Demo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Demo.DataAccess
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCartItem> CartItems { get; set; }
        public DbSet<OrderItemDetails> OrderItems { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }

        //TODO-5+: Improve model seeding to have more models with better descriptions
        //This also would require the concept of the main theme of this shop
        //Although simple, this will be put on a lower priority for now, as it may require input from the Design side
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Fiction", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Drama", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "D Amaze-on" },
                new Company { Id = 2, Name = "Ye old bookshop" },
                new Company { Id = 3, Name = "Tito books" }
            );

            modelBuilder.Entity<Product>().HasData(new Product 
            { 
                Id = 2,
                Title = "Fortune of Time",
                Author = "Billy Spark",
                ISBN = "SWD999901",
                Description = "Lorem ipsum",
                Price = 90,
                Price50 = 85,
                Price100 = 80,
                CategoryId = 3,
            });
        }
    }
}
