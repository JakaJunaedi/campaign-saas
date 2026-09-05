namespace CampaignSaaS.Modules.Campaign.Application.Commands.AddCreatorToRoster;

using FluentValidation;

public class AddCreatorToRosterCommandValidator : AbstractValidator<AddCreatorToRosterCommand>
{
    public AddCreatorToRosterCommandValidator()
    {
        RuleFor(x => x.CampaignId).NotEmpty().WithMessage("Campaign ID is required.");
        RuleFor(x => x.CreatorId).NotEmpty().WithMessage("Creator ID is required.");
        RuleFor(x => x.AgreedRate).GreaterThanOrEqualTo(0).WithMessage("Agreed rate cannot be negative.");
    }
}
