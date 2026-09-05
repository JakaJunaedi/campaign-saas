namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaignStatus;

using CampaignSaaS.Modules.Campaign.Domain.Enums;
using FluentValidation;

public class UpdateCampaignStatusCommandValidator : AbstractValidator<UpdateCampaignStatusCommand>
{
    public UpdateCampaignStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Campaign ID is required.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .IsEnumName(typeof(CampaignStatus), caseSensitive: false).WithMessage("Invalid campaign status specified.");
    }
}
