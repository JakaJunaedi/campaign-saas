namespace CampaignSaaS.Modules.Approval.Application.Commands.SubmitReview;

using CampaignSaaS.Modules.Approval.Domain.Enums;
using FluentValidation;

public class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewCommandValidator()
    {
        RuleFor(x => x.ContentSubmissionId)
            .NotEmpty().WithMessage("ContentSubmissionId is required.");

        RuleFor(x => x.ReviewerId)
            .NotEmpty().WithMessage("ReviewerId is required.");

        RuleFor(x => x.Decision)
            .IsInEnum().WithMessage("A valid review decision is required.");

        RuleFor(x => x.FeedbackNotes)
            .NotEmpty()
            .When(x => x.Decision == ReviewDecision.RevisionRequested)
            .WithMessage("Feedback notes are mandatory when requesting revision.")
            .MaximumLength(2000).WithMessage("Feedback notes cannot exceed 2000 characters.");
    }
}
