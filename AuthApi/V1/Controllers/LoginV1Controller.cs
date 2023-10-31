using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Data.Entities;
using AuthApi.Exceptions;
using AuthApi.V1.Models.Requests;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/login")]
    [ApiExplorerSettings(GroupName = "User Login")]
    public class LoginV1Controller : ControllerBase
    {
        private readonly UserDbContext _userDbContext;
        private readonly SecuritySettings _securitySettings;
        private readonly IValidator<LoginRequestModel> _loginRequestModelValidator;

        public LoginV1Controller(
            UserDbContext userDbContext, 
            IOptions<SecuritySettings> securitySettingsOptions,
            IValidator<LoginRequestModel> loginRequestModelValidator)
        {
            _userDbContext = userDbContext;
            _securitySettings = securitySettingsOptions.Value;
            _loginRequestModelValidator = loginRequestModelValidator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(LoginRequestModel loginRequest)
        {
            ValidationResult validationResult = await _loginRequestModelValidator.ValidateAsync(loginRequest);

            ThrowExceptionIfRequestIsInvalid(validationResult);

            User user = await _userDbContext.Users
                .SingleOrDefaultAsync(u => u.Email.Equals(loginRequest.Email, StringComparison.Ordinal) && u.Password.Equals(loginRequest.Password, StringComparison.Ordinal));

            ThrowExceptionIfUserNotFound(user);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.Secret));

            SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _securitySettings.Issuer,
                audience: _securitySettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: signinCredentials
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) });
        }

        #region Private Methods
        private static void ThrowExceptionIfUserNotFound(User user)
        {
            if (user is null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Unauthorized"
                });
            }
        }

        private static void ThrowExceptionIfRequestIsInvalid(ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occured",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = validationResult.Errors?.FirstOrDefault()?.ErrorMessage
                });
            }
        }
        #endregion
    }
}
