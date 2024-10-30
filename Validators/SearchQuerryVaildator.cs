using FluentValidation;
using TutorHelper.Models;

namespace TutorHelper.Validators
{
    public class SearchQuerryVaildator : AbstractValidator<SearchQuery>
    {
        private static readonly int[] allowedPageSizes = new[] { 10, 20, 30 };

        public SearchQuerryVaildator()
        {
            RuleFor(r => r.PageSize)
                .Must(value => allowedPageSizes.Contains(value))
                .WithMessage($"PageSize must be one of [{string.Join(", ", allowedPageSizes)}]");

            RuleFor(r => r.PageNumber).InclusiveBetween(1, 100)
                .WithMessage("PageNumber must be between 1 and 100");
        }
    }
}