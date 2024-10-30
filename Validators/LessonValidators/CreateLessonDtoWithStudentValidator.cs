using FluentValidation;
using TutorHelper.Models.DtoModels.CreateModels;

namespace TutorHelper.Validators.LessonValidators
{
    public class CreateLessonDtoWithStudentValidator : AbstractValidator<CreateLessonDtoWithStudent>
    {
        public CreateLessonDtoWithStudentValidator()
        {
            RuleFor(x => x.Duration)
                .InclusiveBetween(15, 180)
                .WithMessage("Lekcja musi trwać min. 15 min, maks. 180 min");

            RuleFor(x => x.Price)
                .InclusiveBetween(0, 300)
                .WithMessage("Cena lekcji musi być w przedziale od 0 do 300 zł");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Musisz podać datę lekcji");

            RuleFor(x => x.StudentId)
                 .NotEmpty()
                 .WithMessage("Muisz podać ucznia do którego dodajesz lekcje");

            RuleFor(x => x.RepeatCount)
                       .InclusiveBetween(2, 8)
                       .When(x => x.Repeat)
                       .WithMessage("Jeśli chcesz powtarzalność lekcji, podaj liczbę powtórzeń między 2 a 8.");

            RuleFor(x => x.RepeatCount)
                       .Must(value => value == 0)
                      .When(x => x.Repeat == false)
                      .WithMessage("jeśli nie chcesz powtarzalności lekcji zostaw powtarzalność na default");
        }
    }
}