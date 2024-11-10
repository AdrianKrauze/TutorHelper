using FluentValidation;
using TutorHelper.Models.DtoModels.UpdateModels;

namespace TutorHelper.Validators.StudentValidator
{
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentDtoValidator()
        {
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
                .WithMessage("Invalid StudentConditionId.");

            RuleFor(x => x.ContactTips)
                .Length(0, 100) // Zwiększyłem do 100 dla lepszej elastyczności
                .WithMessage("Wskazówki kontaktu mogą mieć maks. 100 znaków");

            RuleFor(x => x.StudentConditionId)
                .NotEqual("5")
                .WithMessage("Nie możesz zaktualizować studenta dla jednorazowej lekcji.");

          
            RuleFor(x => x.PricePerHour)
                .InclusiveBetween(0, 180)
                .WithMessage("Cena za godzinę musi być większa niż 0.");

            RuleFor(x => x.PricePerDrive)
              .Empty()
              .InclusiveBetween(0, 100)
              .When(x => x.LessonPlaceId != "2")
              .WithMessage("Nie podawaj ceny dojazdu jak to uczeń bez dojazdu");
        }
    }
}