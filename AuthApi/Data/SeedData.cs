using AuthApi.Data.Entities;

namespace AuthApi.Data
{
    public static class SeedData
    {
        public static void Initialize(UserDbContext userDbContext)
        {
            SeedUsers(userDbContext);
        }

        private static void SeedUsers(UserDbContext userDbContext)
        {
            if (!userDbContext.Users.Any())
            {
                var users = new List<User>
                {
                    new User()
                    {
                        Id = 1,
                        Email = "berkAdmin@mail.com",
                        Password = "berkAdmin",
                        Role = Enums.UserRole.Administrator,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User()
                    {
                        Id = 2,
                        Email = "berkEditor@mail.com",
                        Password = "berkEditor",
                        Role = Enums.UserRole.Editor,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User()
                    {
                        Id = 3,
                        Email = "berkReader@mail.com",
                        Password = "berkReader",
                        Role = Enums.UserRole.Reader,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                userDbContext.Users.AddRange(users);
                userDbContext.SaveChanges();
            }
        }
    }
}
