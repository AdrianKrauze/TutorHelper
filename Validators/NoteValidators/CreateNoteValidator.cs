using FluentValidation;

namespace TutorHelper.Models.DtoModels.CreateModels
{
    public class CreateNoteValidator : AbstractValidator<CreateNoteDto>
    {
        public CreateNoteValidator()
        {
            RuleFor(x => x.Content)
                .MaximumLength(300)
                .WithMessage("Content can't be longer than 300 characters.");
        }
    }
}
