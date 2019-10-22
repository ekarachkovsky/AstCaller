using AstCaller.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace AstCaller.Classes
{
    public class SeedDatabase
    {
        private MainContext _context;

        public SeedDatabase(MainContext context)
        {
            _context = context;
        }

        public void CreateAdminUser()
        {
            var user = new User
            {
                Login = "admin",
                Password = "12345678",
                Fullname = "admin"
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
