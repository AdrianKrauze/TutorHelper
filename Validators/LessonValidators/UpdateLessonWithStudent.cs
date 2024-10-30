using FluentValidation;
using TutorHelper.Models.DtoModels.UpdateModels;

namespace TutorHelper.Validators.LessonValidators
{
    public class UpdateLessonWithStudentValidator : AbstractValidator<UpdateLessonWithStudentDto>
    {
        public UpdateLessonWithStudentValidator()
        {
            RuleFor(x => x.Duration)
                .InclusiveBetween(15, 180)
                .When(x => x.Duration.HasValue)
                .WithMessage("Lekcja musi trwać min. 15 min, maks. 180 min");

            RuleFor(x => x.Price)
               .InclusiveBetween(0, 300)
                .When(x => x.Price.HasValue)
               .WithMessage("Cena lekcji musi być w przedziale od 0 do 300 zł");
        }
    }
}