namespace CampaignSaaS.Modules.Identity.Application.Commands.ImpersonateTenant;

using FluentValidation;

public class ImpersonateTenantCommandValidator : AbstractValidator<ImpersonateTenantCommand>
{
    public ImpersonateTenantCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();
    }
}
