using FluentValidation;
using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.Validators.LessonValidators
{
    public class CreateLessonDtoWoStudentValidator : AbstractValidator<CreateLessonDtoWoStudent>
    {
        public CreateLessonDtoWoStudentValidator()
        {
            RuleFor(x => x.Duration)
                .NotEmpty()
                .InclusiveBetween(15, 180)
                .WithMessage("Lekcja musi trwać min. 15 min, maks. 180 min");

            RuleFor(x => x.Price)
                .NotEmpty()
                .InclusiveBetween(0, 300)
                .WithMessage("Cena lekcji musi być w przedziale od 0 do 300 zł");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Musisz podać datę lekcji");

            RuleFor(x => x.RepeatCount)
                .InclusiveBetween(2, 8)
                .When(x => x.Repeat)
                .WithMessage("Jeśli chcesz powtarzalność lekcji, podaj liczbę powtórzeń między 2 a 8. Inaczej dodaj lekcje pojedynczo");

            RuleFor(x => x.PushTimeBeforeLesson)
                .InclusiveBetween(0, 1440)
                .When(x => x.PushBoolean)
                .WithMessage("Jeżeli chcesz komunikat przed lekcją, podaj na ile minut przed, maks. 1440 min (24 godziny)");

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

            RuleFor(x => x.ContactTips)
                .Length(0, 100)
                .WithMessage("Szczegóły kontaktu mogą mieć maks 100 znaków");

            RuleFor(x => x.StudentFirstName)
                .NotEmpty()
                .WithMessage("Podaj imię ucznia");

            RuleFor(x => x.StudentLastName)
                .NotEmpty()
                .WithMessage("Podaj nazwisko ucznia");
        }
    }
}