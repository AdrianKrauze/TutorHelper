using FluentValidation;
using TutorHelper.Models.IdentityModels;

namespace TutorHelper.Validators.AccountModelsValidators
{
    public class ChangePasswordValidator:AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {

            RuleFor(x => x.NewPassword)
               .NotEmpty().WithMessage("Hasło jest wymagane.")
               .MinimumLength(6).WithMessage("Hasło musi mieć co najmniej 6 znaków.")
               .Matches("[A-Z]").WithMessage("Hasło musi zawierać przynajmniej jedną dużą literę.")
               .Matches("[a-z]").WithMessage("Hasło musi zawierać przynajmniej jedną małą literę.")
               .Matches("[0-9]").WithMessage("Hasło musi zawierać przynajmniej jedną cyfrę.")
               .Matches("[^a-zA-Z0-9]").WithMessage("Hasło musi zawierać przynajmniej jeden znak specjalny.");

            RuleFor(x => x.NewConfirmPassword)
            .NotEmpty().WithMessage("Hasło jest wymagane.")
            .MinimumLength(6).WithMessage("Hasło musi mieć co najmniej 6 znaków.")
            .Matches("[A-Z]").WithMessage("Hasło musi zawierać przynajmniej jedną dużą literę.")
            .Matches("[a-z]").WithMessage("Hasło musi zawierać przynajmniej jedną małą literę.")
            .Matches("[0-9]").WithMessage("Hasło musi zawierać przynajmniej jedną cyfrę.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Hasło musi zawierać przynajmniej jeden znak specjalny.")
            .Equal(x => x.NewPassword).WithMessage("Hasła nie są identyczne.");
        }
    }
}
