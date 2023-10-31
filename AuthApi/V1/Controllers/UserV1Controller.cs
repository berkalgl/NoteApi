using AuthApi.Data;
using AuthApi.Data.Entities;
using AuthApi.Data.Enums;
using AuthApi.Exceptions;
using AuthApi.V1.Models.Enums;
using AuthApi.V1.Models.Requests;
using AuthApi.V1.Models.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [ApiExplorerSettings(GroupName = "Users")]
    [Authorize]
    public class UserV1Controller : Controller
    {
        private readonly UserDbContext _dbContext;
        private readonly IValidator<UpdateUserRequestModel> _updateUserRequestModelValidator;

        public UserV1Controller(
            UserDbContext dbContext,
            IValidator<UpdateUserRequestModel> updateUserRequestModelValidator)
        {
            _dbContext = dbContext;
            _updateUserRequestModelValidator = updateUserRequestModelValidator;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(i => i.Id == id);

            ThrowExceptionIfUserNotFound(user);

            UserResponseModel response = new()
            {
                Id = user.Id,
                Email = user.Email,
                Role = (UserRoleEnum)user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] UpdateUserRequestModel request)
        {
            ValidationResult validationResult = await _updateUserRequestModelValidator.ValidateAsync(request);

            ThrowExceptionIfRequestIsInvalid(validationResult);

            User user = await _dbContext.Users.FirstOrDefaultAsync(i => i.Id == id);

            ThrowExceptionIfUserNotFound(user);
            
            user.Email = request.Email;
            user.Password = request.Password;
            user.Role = (UserRole)request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<UserResponseModel> response = await _dbContext.Users
                .AsNoTracking()
                .Select(i => new UserResponseModel
                {
                    Id = i.Id,
                    Email = i.Email,
                    Password = i.Password,
                    Role = (UserRoleEnum)i.Role,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();

            return Ok(response);
        }

        #region Private Methods

        private static void ThrowExceptionIfUserNotFound(User user)
        {
            if (user is null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "User is not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = "User is not found"
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
