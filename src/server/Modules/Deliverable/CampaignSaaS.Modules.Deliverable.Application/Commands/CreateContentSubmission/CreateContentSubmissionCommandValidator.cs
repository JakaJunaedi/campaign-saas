namespace CampaignSaaS.Modules.Deliverable.Application.Commands.CreateContentSubmission;

using FluentValidation;

public class CreateContentSubmissionCommandValidator : AbstractValidator<CreateContentSubmissionCommand>
{
    public CreateContentSubmissionCommandValidator()
    {
        RuleFor(x => x.DeliverableId)
            .NotEmpty().WithMessage("DeliverableId is required.");

        RuleFor(x => x.MediaObjectKey)
            .NotEmpty().WithMessage("MediaObjectKey is required.")
            .MaximumLength(500).WithMessage("MediaObjectKey cannot exceed 500 characters.");

        RuleFor(x => x.MediaFileName)
            .NotEmpty().WithMessage("MediaFileName is required.")
            .MaximumLength(255).WithMessage("MediaFileName cannot exceed 255 characters.");

        RuleFor(x => x.MediaFileSize)
            .GreaterThan(0).WithMessage("MediaFileSize must be greater than 0 bytes.");
    }
}

