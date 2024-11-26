using FluentValidation;
using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.Validators
{
    public class CreateEmailDtoValidator : AbstractValidator<CreateEmailDto>
    {
        public CreateEmailDtoValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Imię jest wymagane.")
                .MaximumLength(50).WithMessage("Imię nie może mieć więcej niż 50 znaków.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Nazwisko jest wymagane.")
                .MaximumLength(50).WithMessage("Nazwisko nie może mieć więcej niż 50 znaków.");

           
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Adres e-mail jest wymagany.")
                .EmailAddress().WithMessage("Podano nieprawidłowy adres e-mail.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Numer telefonu jest wymagany.")
                .Matches(@"^\+48\d{9}$").WithMessage("Numer telefonu musi być w formacie +48 i zawierać 9 cyfr.");

            
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Treść wiadomości nie może być pusta.")
                .MaximumLength(1000).WithMessage("Treść wiadomości nie może przekraczać 1000 znaków.");

           
            RuleFor(x => x.EmailCase)
                .IsInEnum().WithMessage("Nieprawidłowa wartość dla pola EmailCase.");
        }
    }
}
