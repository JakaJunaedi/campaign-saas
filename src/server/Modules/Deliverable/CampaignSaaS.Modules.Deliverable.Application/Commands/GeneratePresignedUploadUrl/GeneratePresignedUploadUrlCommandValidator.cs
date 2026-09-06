namespace CampaignSaaS.Modules.Deliverable.Application.Commands.GeneratePresignedUploadUrl;

using FluentValidation;

public class GeneratePresignedUploadUrlCommandValidator : AbstractValidator<GeneratePresignedUploadUrlCommand>
{
    public GeneratePresignedUploadUrlCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required.")
            .MaximumLength(255).WithMessage("FileName cannot exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("FileSize must be greater than 0 bytes.")
            .LessThanOrEqualTo(500L * 1024 * 1024).WithMessage("FileSize cannot exceed 500MB.");
    }
}

