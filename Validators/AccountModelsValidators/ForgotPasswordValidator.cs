using FluentValidation;
using TutorHelper.Models.IdentityModels;

namespace TutorHelper.Validators.AccountModelsValidators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email jest wymagany.")
               .EmailAddress().WithMessage("Nieprawidłowy format adresu email.");
        }
    }
}
