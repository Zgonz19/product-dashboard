using FluentValidation;
using ProductAPI.DTOs;

namespace ProductAPI.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name must be 100 characters or fewer");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be 500 characters or fewer");
        }
    }
}
