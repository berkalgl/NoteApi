using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteApi.Data;
using NoteApi.Data.Entities;
using NoteApi.Exceptions;
using NoteApi.V1.Models.Requests;
using NoteApi.V1.Models.Responses;

namespace NoteApi.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users/{userId}/notes")]
    [ApiExplorerSettings(GroupName = "User Notes")]
    public class UserNoteV1Controller : Controller
    {
        private readonly NoteDbContext _dbContext;
        private readonly IValidator<CreateNoteRequestModel> _createNoteRequestModelValidator;
        private readonly IValidator<UpdateNoteRequestModel> _updateNoteRequestModelValidator;

        public UserNoteV1Controller(
            NoteDbContext dbContext, 
            IValidator<CreateNoteRequestModel> createNoteRequestModelValidator, 
            IValidator<UpdateNoteRequestModel> updateNoteRequestModelValidator)
        {
            _dbContext = dbContext;
            _createNoteRequestModelValidator = createNoteRequestModelValidator;
            _updateNoteRequestModelValidator = updateNoteRequestModelValidator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromRoute]int userId, [FromBody]CreateNoteRequestModel request)
        {
            ValidationResult validationResult = await _createNoteRequestModelValidator.ValidateAsync(request);

            ThrowExceptionIfRequestIsInvalid(validationResult);

            Note note = new()
            {
                UserId = userId,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow,
            };

            await _dbContext.Notes.AddAsync(note);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { userId = note.UserId, id = note.Id }, note);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponseModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromRoute] int userId, [FromRoute] int id)
        {
            Note note = await _dbContext.Notes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            ThrowExceptionIfNoteNotFound(note);

            NoteResponseModel response = new()
            {
                Id = note.Id,
                UserId = note.UserId,
                Text = note.Text,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int userId, [FromRoute] int id)
        {
            Note note = await _dbContext.Notes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (note is null)
            {
                return NoContent();
            }

            _dbContext.Remove(note);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromRoute] int userId, [FromRoute] int id, [FromBody] UpdateNoteRequestModel request)
        {
            ValidationResult validationResult = await _updateNoteRequestModelValidator.ValidateAsync(request);

            ThrowExceptionIfRequestIsInvalid(validationResult);

            Note note = await _dbContext.Notes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            ThrowExceptionIfNoteNotFound(note);

            note.Text = request.Text;
            note.UpdatedAt = DateTime.UtcNow;

            _dbContext.Update(note);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponseModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromRoute]int userId)
        {
            IEnumerable<NoteResponseModel> response = await _dbContext.Notes
                .AsNoTracking()
                .Where(i => i.UserId == userId)
                .Select(i => new NoteResponseModel
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    Text = i.Text,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();

            return Ok(response);
        }

        #region Private Methods

        private static void ThrowExceptionIfNoteNotFound(Note note)
        {
            if (note is null)
            {
                throw new ProblemDetailsException(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Note is not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = "Note is not found"
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
