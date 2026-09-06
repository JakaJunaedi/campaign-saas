namespace CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;

using FluentValidation;

public class GenerateCampaignReportCommandValidator : AbstractValidator<GenerateCampaignReportCommand>
{
    public GenerateCampaignReportCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("CampaignId is required.");
    }
}
