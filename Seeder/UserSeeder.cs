using Microsoft.EntityFrameworkCore;
using Reddit_App.Models;

namespace Reddit_App.Seeder
{
    public class UserSeeder
    {
        private readonly ModelBuilder _modelBuilder;
        
        public UserSeeder(ModelBuilder builder)
        {
            _modelBuilder = builder;
        }

        public void SeedData()
        {
            _modelBuilder.Entity<users>().HasData(new users
            {
                UserID = 1,
                UserName = "admin",
                PassWord = "admin",
                Email = "admin@gmail.com",
                DateOfBirth = new DateTime(2003, 11, 02)
            });
        }
    }
}
