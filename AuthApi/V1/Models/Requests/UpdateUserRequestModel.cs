using AuthApi.V1.Models.Enums;

namespace AuthApi.V1.Models.Requests
{
    public class UpdateUserRequestModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public UserRoleEnum Role { get; set; }
    }
}
