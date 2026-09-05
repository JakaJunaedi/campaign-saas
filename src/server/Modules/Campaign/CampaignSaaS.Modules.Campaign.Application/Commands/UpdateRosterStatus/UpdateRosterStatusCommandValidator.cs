namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateRosterStatus;

using CampaignSaaS.Modules.Campaign.Domain.Enums;
using FluentValidation;

public class UpdateRosterStatusCommandValidator : AbstractValidator<UpdateRosterStatusCommand>
{
    public UpdateRosterStatusCommandValidator()
    {
        RuleFor(x => x.CampaignId).NotEmpty().WithMessage("Campaign ID is required.");
        RuleFor(x => x.CreatorId).NotEmpty().WithMessage("Creator ID is required.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .IsEnumName(typeof(CampaignCreatorStatus), caseSensitive: false).WithMessage("Invalid roster status specified.");

        RuleFor(x => x.AgreedRate)
            .GreaterThanOrEqualTo(0).When(x => x.AgreedRate.HasValue)
            .WithMessage("Agreed rate cannot be negative.");
    }
}
