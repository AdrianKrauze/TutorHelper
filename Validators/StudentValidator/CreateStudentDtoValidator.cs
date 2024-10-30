using FluentValidation;
using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.Validators.StudentValidator
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.PricePerHour)
               .InclusiveBetween(0, 180)
               .WithMessage("Cena za godzinę musi być większa niż 0.");
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(1, 50)
                .WithMessage("Podaj imię, maksymalnie 50 znaków");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(1, 50)
                .WithMessage("Podaj nazwisko, maksymalnie 50 znaków");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Numer telefonu nie może być pusty.")
                .Matches(@"^\+48[1-9]\d{8}$")
                .WithMessage("Numer telefonu musi być w poprawnym formacie");

            RuleFor(x => x.EduStageId)
                .NotEmpty()
                .Must(id => ValidationConstants.EduStageIds.Contains(id))
                .WithMessage("Invalid EduStageId.");

            RuleFor(x => x.SubjectId)
                .NotEmpty()
                .Must(id => ValidationConstants.SubjectIds.Contains(id))
                .WithMessage("Invalid SubjectId.");

            RuleFor(x => x.LessonPlaceId)
                .NotEmpty()
                .Must(id => ValidationConstants.LessonPlaceIds.Contains(id))
                .WithMessage("Invalid LessonPlaceId.");

            RuleFor(x => x.StudentConditionId)
                .NotEmpty()
                .Must(id => ValidationConstants.StudentConditionIds.Contains(id))
                .WithMessage("Invalid StudentCondition.");

            RuleFor(x => x.ContactTips)
                .Length(0, 100)
                .WithMessage("Wskazówki kontaktu mogą mieć maks 100 znaków");

            RuleFor(x => x.StudentConditionId)
                .NotEqual("5")
                .WithMessage("Nie możesz stworzyć studenta dla jednorazowej lekcji.");

            RuleFor(x => x.PricePerDrive)
                .NotEmpty()
                .InclusiveBetween(0, 100)
                .When(x => x.LessonPlaceId == "2")
                .WithMessage("Jezeli uczen na dojazd podaj cene dojazdu");

            RuleFor(x => x.PricePerDrive)
               .Empty()
               .InclusiveBetween(0, 100)
               .When(x => x.LessonPlaceId != "2")
               .WithMessage("Nie podawaj ceny dojazdu jak to uczeń bez dojazdu");
        }
    }
}