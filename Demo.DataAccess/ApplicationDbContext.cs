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

        // Seeds content
        // DBInitializer seeds roles and the initial admin account
        // This is more about products, companies and categories
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Workspace", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Tech Accessories", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Organization", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Travel", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Home Essentials", DisplayOrder = 5 }
            );

            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Apex Office Solutions" },
                new Company { Id = 2, Name = "HarborTech Wholesale" },
                new Company { Id = 3, Name = "ModuLiving Distributors" },
                new Company { Id = 4, Name = "TransitPro Supply" },
                new Company { Id = 5, Name = "Hearth & Home Trade" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Arc Monitor Stand",
                    Author = "Northstar Studio",
                    ISBN = "NSS-WS-001",
                    Description = "Elevated aluminum monitor stand designed to create a cleaner desk setup and improve everyday screen ergonomics.",
                    Price = 89,
                    Price100 = 79,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 2,
                    Title = "Ridge Desk Lamp",
                    Author = "Luma Works",
                    ISBN = "NSS-WS-002",
                    Description = "Minimal LED desk lamp with an adjustable neck and warm ambient lighting for focused work sessions.",
                    Price = 79,
                    Price100 = 69,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 3,
                    Title = "Atlas Desk Mat",
                    Author = "Atlas Goods",
                    ISBN = "NSS-WS-003",
                    Description = "Soft-touch desk mat that anchors keyboards, mice, and notebooks while adding a refined layer to the workspace.",
                    Price = 35,
                    Price100 = 29,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 4,
                    Title = "Pivot Laptop Riser",
                    Author = "Pivot Labs",
                    ISBN = "NSS-WS-004",
                    Description = "Foldable laptop riser built for better posture, cleaner airflow, and flexible work-from-home setups.",
                    Price = 59,
                    Price100 = 52,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 5,
                    Title = "Harbor Wireless Keyboard",
                    Author = "Harbor Works",
                    ISBN = "NSS-TA-001",
                    Description = "Slim wireless keyboard with a clean layout and quiet typing feel, designed for modern desk setups.",
                    Price = 99,
                    Price100 = 89,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 6,
                    Title = "Harbor Wireless Mouse",
                    Author = "Harbor Works",
                    ISBN = "NSS-TA-002",
                    Description = "Compact wireless mouse with an ergonomic silhouette and smooth tracking for daily productivity.",
                    Price = 49,
                    Price100 = 43,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 7,
                    Title = "Signal USB-C Hub",
                    Author = "Signal Labs",
                    ISBN = "NSS-TA-003",
                    Description = "Multi-port USB-C hub that expands laptop connectivity with a sleek footprint for travel or desk use.",
                    Price = 69,
                    Price100 = 61,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 8,
                    Title = "Dockline Charging Station",
                    Author = "Dockline Systems",
                    ISBN = "NSS-TA-004",
                    Description = "Streamlined charging station for phones, earbuds, and accessories, built to reduce cable clutter.",
                    Price = 79,
                    Price100 = 69,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 9,
                    Title = "Fold Storage Box Set",
                    Author = "Fold Home",
                    ISBN = "NSS-OR-001",
                    Description = "Set of collapsible storage boxes that helps keep shelves, closets, and work areas visually organized.",
                    Price = 39,
                    Price100 = 33,
                    CategoryId = 3,
                },
                new Product
                {
                    Id = 10,
                    Title = "Grid Drawer Organizer",
                    Author = "Grid House",
                    ISBN = "NSS-OR-002",
                    Description = "Modular drawer organizer designed for office supplies, personal accessories, and small everyday items.",
                    Price = 29,
                    Price100 = 25,
                    CategoryId = 3,
                },
                new Product
                {
                    Id = 11,
                    Title = "Stack Document Tray",
                    Author = "Stack Office",
                    ISBN = "NSS-OR-003",
                    Description = "Layered document tray with a clean silhouette for sorting paperwork, notebooks, and incoming mail.",
                    Price = 34,
                    Price100 = 28,
                    CategoryId = 3,
                },
                new Product
                {
                    Id = 12,
                    Title = "Rail Wall Hook Rack",
                    Author = "Rail Utility",
                    ISBN = "NSS-OR-004",
                    Description = "Simple wall-mounted hook rack for entryways, offices, and utility spaces that need practical storage.",
                    Price = 32,
                    Price100 = 26,
                    CategoryId = 3,
                },
                new Product
                {
                    Id = 13,
                    Title = "Transit Carry Backpack",
                    Author = "Transit Co.",
                    ISBN = "NSS-TR-001",
                    Description = "Daily carry backpack with a streamlined profile, laptop compartment, and versatile storage for commuting.",
                    Price = 129,
                    Price100 = 115,
                    CategoryId = 4,
                },
                new Product
                {
                    Id = 14,
                    Title = "Harbor Laptop Sleeve",
                    Author = "Harbor Works",
                    ISBN = "NSS-TR-002",
                    Description = "Protective laptop sleeve with a minimalist exterior and soft inner lining for daily transport.",
                    Price = 45,
                    Price100 = 39,
                    CategoryId = 4,
                },
                new Product
                {
                    Id = 15,
                    Title = "Roam Travel Pouch",
                    Author = "Roam Supply",
                    ISBN = "NSS-TR-003",
                    Description = "Compact organizer pouch for chargers, passports, toiletries, and other travel essentials.",
                    Price = 28,
                    Price100 = 24,
                    CategoryId = 4,
                },
                new Product
                {
                    Id = 16,
                    Title = "Summit Insulated Bottle",
                    Author = "Summit Goods",
                    ISBN = "NSS-TR-004",
                    Description = "Insulated stainless steel bottle that keeps drinks at the right temperature through commutes and day trips.",
                    Price = 33,
                    Price100 = 27,
                    CategoryId = 4,
                },
                new Product
                {
                    Id = 17,
                    Title = "Vale Ambient Lamp",
                    Author = "Vale Living",
                    ISBN = "NSS-HE-001",
                    Description = "Soft ambient table lamp created to bring warmth and calm lighting into bedrooms, living rooms, and reading corners.",
                    Price = 74,
                    Price100 = 64,
                    CategoryId = 5,
                },
                new Product
                {
                    Id = 18,
                    Title = "Hearth Utility Tray",
                    Author = "Hearth Studio",
                    ISBN = "NSS-HE-002",
                    Description = "Everyday catch-all tray for keys, wallets, candles, and small essentials that tend to gather around the home.",
                    Price = 26,
                    Price100 = 22,
                    CategoryId = 5,
                },
                new Product
                {
                    Id = 19,
                    Title = "Drift Throw Blanket",
                    Author = "Drift Home",
                    ISBN = "NSS-HE-003",
                    Description = "Textured throw blanket designed to add warmth, softness, and a lived-in feel to sofas and beds.",
                    Price = 49,
                    Price100 = 43,
                    CategoryId = 5,
                },
                new Product
                {
                    Id = 20,
                    Title = "Grove Reed Diffuser",
                    Author = "Grove Living",
                    ISBN = "NSS-HE-004",
                    Description = "Subtle reed diffuser with a clean vessel and balanced fragrance profile for calm interior spaces.",
                    Price = 31,
                    Price100 = 26,
                    CategoryId = 5,
                }
            );
        }
    }
}
