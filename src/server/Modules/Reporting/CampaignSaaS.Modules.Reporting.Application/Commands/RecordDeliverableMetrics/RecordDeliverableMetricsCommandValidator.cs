namespace CampaignSaaS.Modules.Reporting.Application.Commands.RecordDeliverableMetrics;

using FluentValidation;

public class RecordDeliverableMetricsCommandValidator : AbstractValidator<RecordDeliverableMetricsCommand>
{
    public RecordDeliverableMetricsCommandValidator()
    {
        RuleFor(x => x.DeliverableId)
            .NotEmpty().WithMessage("DeliverableId is required.");

        RuleFor(x => x.Reach)
            .GreaterThanOrEqualTo(0).WithMessage("Reach must be greater than or equal to 0.");

        RuleFor(x => x.Impressions)
            .GreaterThanOrEqualTo(0).WithMessage("Impressions must be greater than or equal to 0.");

        RuleFor(x => x.Views)
            .GreaterThanOrEqualTo(0).WithMessage("Views must be greater than or equal to 0.");

        RuleFor(x => x.Likes)
            .GreaterThanOrEqualTo(0).WithMessage("Likes must be greater than or equal to 0.");

        RuleFor(x => x.Comments)
            .GreaterThanOrEqualTo(0).WithMessage("Comments must be greater than or equal to 0.");

        RuleFor(x => x.Shares)
            .GreaterThanOrEqualTo(0).WithMessage("Shares must be greater than or equal to 0.");

        RuleFor(x => x.Clicks)
            .GreaterThanOrEqualTo(0).WithMessage("Clicks must be greater than or equal to 0.");
    }
}
