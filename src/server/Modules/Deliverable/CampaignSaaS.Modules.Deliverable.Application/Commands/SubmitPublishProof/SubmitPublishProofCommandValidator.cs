namespace CampaignSaaS.Modules.Deliverable.Application.Commands.SubmitPublishProof;

using FluentValidation;

public class SubmitPublishProofCommandValidator : AbstractValidator<SubmitPublishProofCommand>
{
    public SubmitPublishProofCommandValidator()
    {
        RuleFor(x => x.DeliverableId)
            .NotEmpty().WithMessage("DeliverableId is required.");

        RuleFor(x => x.LiveUrl)
            .NotEmpty().WithMessage("LiveUrl is required.")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("LiveUrl must be a valid absolute URL.");
    }
}

