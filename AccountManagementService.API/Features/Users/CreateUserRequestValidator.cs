using FluentValidation;

namespace AccountManagementService.API.Features.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be atleast 2characters")
                .MaximumLength(10).WithMessage("Name cannot exceed 10 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter valid email address");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 50).WithMessage("Age must be between 18 and 50");
        }
    }
}
