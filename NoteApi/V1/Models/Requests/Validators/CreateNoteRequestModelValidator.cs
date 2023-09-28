using FluentValidation;

namespace NoteApi.V1.Models.Requests.Validators
{
    public class CreateNoteRequestModelValidator : AbstractValidator<CreateNoteRequestModel>
    {
        public CreateNoteRequestModelValidator() 
        {
            RuleFor(x => x.Text)
                .NotEmpty();
        }
    }
}