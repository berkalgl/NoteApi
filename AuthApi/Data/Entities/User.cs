using AuthApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required()]
        public string Email { get; set; }

        [Required()]
        public string Password { get; set; }

        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
