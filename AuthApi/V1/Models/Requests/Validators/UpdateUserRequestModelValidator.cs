using FluentValidation;

namespace AuthApi.V1.Models.Requests.Validators
{
    public class UpdateUserRequestModelValidator : AbstractValidator<UpdateUserRequestModel>
    {
        public UpdateUserRequestModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.Role)
                .NotNull()
                .IsInEnum();
        }
    }
}
