using FluentValidation;
using TutorHelper.Models.DtoModels.UpdateModels;

namespace TutorHelper.Validators.LessonValidators
{
    public class UpdateLessonWithoutStudentValidator : AbstractValidator<UpdateLessonWithoutStudentDto>
    {
        public UpdateLessonWithoutStudentValidator()
        {
            RuleFor(x => x.Duration)
                .InclusiveBetween(15, 180)
                .WithMessage("Lekcja musi trwać min. 15 min, maks. 180 min");

            RuleFor(x => x.Price)
                .InclusiveBetween(0, 300)
                .WithMessage("Cena lekcji musi być w przedziale od 0 do 300 zł");

            /*RuleFor(x => x.PushTimeBeforeLesson)
                  .InclusiveBetween(0, 1440)
                 .When(x => x.PushBoolean.HasValue && x.PushBoolean.Value)
                  .WithMessage("Jeżeli chcesz komunikat przed lekcją, podaj na ile minut przed, maks. 1440 min (24 godziny)");
           */
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+48[1-9]\d{8}$")
                .WithMessage("Numer telefonu musi być w poprawnym formacie");

            RuleFor(x => x.EduStageId)
                .Must(id => ValidationConstants.EduStageIds.Contains(id))
                .When(x => !string.IsNullOrEmpty(x.EduStageId))
                .WithMessage("Invalid EduStageId.");

            RuleFor(x => x.SubjectId)
                .Must(id => ValidationConstants.SubjectIds.Contains(id))
                .When(x => !string.IsNullOrEmpty(x.SubjectId))
                .WithMessage("Invalid SubjectId.");

            RuleFor(x => x.LessonPlaceId)
                .Must(id => ValidationConstants.LessonPlaceIds.Contains(id))
                .When(x => !string.IsNullOrEmpty(x.LessonPlaceId))
                .WithMessage("Invalid LessonPlaceId.");

            RuleFor(x => x.ContactTips)
                .Length(0, 100)
                .WithMessage("Szczegóły kontaktu mogą mieć maksymalnie 100 znaków");

            RuleFor(x => x.StudentFirstName)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.StudentLastName)) // Assuming first name is required if last name is provided
                .WithMessage("Podaj imię ucznia");

            RuleFor(x => x.StudentLastName)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.StudentFirstName)) // Assuming last name is required if first name is provided
                .WithMessage("Podaj nazwisko ucznia");
        }
    }
}