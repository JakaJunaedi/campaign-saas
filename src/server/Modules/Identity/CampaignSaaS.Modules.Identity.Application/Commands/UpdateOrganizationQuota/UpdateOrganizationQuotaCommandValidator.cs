namespace CampaignSaaS.Modules.Identity.Application.Commands.UpdateOrganizationQuota;

using FluentValidation;

public class UpdateOrganizationQuotaCommandValidator : AbstractValidator<UpdateOrganizationQuotaCommand>
{
    public UpdateOrganizationQuotaCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.StorageQuotaBytes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Storage quota cannot be negative.");

        RuleFor(x => x.MaxActiveCampaigns)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Max active campaigns cannot be negative.");
    }
}
