namespace CampaignSaaS.Modules.Creator.Application.Commands.UpdateCreatorStatus;

using CampaignSaaS.Modules.Creator.Domain.Enums;
using FluentValidation;

public class UpdateCreatorStatusCommandValidator : AbstractValidator<UpdateCreatorStatusCommand>
{
    public UpdateCreatorStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Creator ID is required.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .IsEnumName(typeof(CreatorStatus), caseSensitive: false).WithMessage("Invalid creator status specified.");
    }
}
