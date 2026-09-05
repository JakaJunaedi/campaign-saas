namespace CampaignSaaS.Modules.Creator.Application.Commands.CreateCreator;

using CampaignSaaS.Modules.Creator.Domain.Enums;
using FluentValidation;

public class CreateCreatorCommandValidator : AbstractValidator<CreateCreatorCommand>
{
    public CreateCreatorCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

        RuleFor(x => x.Niche)
            .NotEmpty().WithMessage("Niche is required.")
            .MaximumLength(100).WithMessage("Niche must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("A valid email address is required.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters.");

        RuleForEach(x => x.SocialAccounts).ChildRules(social =>
        {
            social.RuleFor(s => s.Platform)
                .NotEmpty().WithMessage("Platform is required.")
                .IsEnumName(typeof(PlatformType), caseSensitive: false).WithMessage("Invalid platform specified.");

            social.RuleFor(s => s.Handle)
                .NotEmpty().WithMessage("Handle is required.")
                .MaximumLength(100).WithMessage("Handle must not exceed 100 characters.");

            social.RuleFor(s => s.FollowerCount)
                .GreaterThanOrEqualTo(0).WithMessage("Follower count cannot be negative.");
        });
    }
}
