using FluentValidation;

namespace NoteApi.V1.Models.Requests.Validators
{
    public class UpdateNoteRequestModelValidator : AbstractValidator<UpdateNoteRequestModel>
    {
        public UpdateNoteRequestModelValidator() 
        {
            RuleFor(x => x.Text)
                .NotEmpty();
        }
    }
}