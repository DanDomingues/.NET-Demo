
using Demo.Models;
using Demo.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Demo.DataAccess
{
    public class DbInitializer(
        UserManager<IdentityUser> um, 
        RoleManager<IdentityRole> rm, 
        ApplicationDbContext db) : IDbInitializer
    {
        public void Initialize()
        {
            try
            {
                if(db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }

            if(!rm.RoleExistsAsync(SD.ROLE_USER_ADMIN).GetAwaiter().GetResult())
            {
                rm.CreateAsync(new(SD.ROLE_USER_ADMIN)).GetAwaiter().GetResult();
                rm.CreateAsync(new(SD.ROLE_USER_COMPANY)).GetAwaiter().GetResult();
                rm.CreateAsync(new(SD.ROLE_USER_CUSTOMER)).GetAwaiter().GetResult();
                rm.CreateAsync(new(SD.ROLE_USER_EMPLOYEE)).GetAwaiter().GetResult();

                um.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@email.com",
                    Email = "admin@email.com",
                    FirstName = "John",
                    LastName = "Smith",
                    PhoneNumber = "12345678",
                    StreetAddress = "Test Avenue",
                    State = "IP",
                    PostalCode = "12345",
                    City = "New York"
                },
                "Admin123*").GetAwaiter().GetResult();

                var userInDb = 
                    db.ApplicationUsers.FirstOrDefault(u => u.UserName != null && u.UserName.Equals("admin@email.com")) 
                    ?? throw new InvalidOperationException("Admin user was not created successfully.");
                    
                um.AddToRoleAsync(userInDb, SD.ROLE_USER_ADMIN).GetAwaiter().GetResult();
                var roles = um.GetRolesAsync(userInDb).GetAwaiter().GetResult();
            }
        }
    }
}
