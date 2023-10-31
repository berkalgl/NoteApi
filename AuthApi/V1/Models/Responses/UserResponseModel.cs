using AuthApi.V1.Models.Enums;

namespace AuthApi.V1.Models.Responses
{
    public class UserResponseModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public UserRoleEnum Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
