using TutorHelper.Models.IdentityModels;
using FluentValidation;

namespace TutorHelper.Validators.AccountModelsValidators
{

    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email jest wymagany.")
                .EmailAddress().WithMessage("Nieprawidłowy format adresu email.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Hasło jest wymagane.")
                .MinimumLength(6).WithMessage("Hasło musi mieć co najmniej 6 znaków.")
                .Matches("[A-Z]").WithMessage("Hasło musi zawierać przynajmniej jedną dużą literę.")
                .Matches("[a-z]").WithMessage("Hasło musi zawierać przynajmniej jedną małą literę.")
                .Matches("[0-9]").WithMessage("Hasło musi zawierać przynajmniej jedną cyfrę.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Hasło musi zawierać przynajmniej jeden znak specjalny.");

            RuleFor(x => x.RememberMe)
                .NotNull(); // Prawda lub fałsz, nie jest wymagana dodatkowa walidacja
        }
    }
}