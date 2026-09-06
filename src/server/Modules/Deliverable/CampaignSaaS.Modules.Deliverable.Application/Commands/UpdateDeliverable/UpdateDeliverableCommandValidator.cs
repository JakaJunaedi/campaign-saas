namespace CampaignSaaS.Modules.Deliverable.Application.Commands.UpdateDeliverable;

using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using FluentValidation;

public class UpdateDeliverableCommandValidator : AbstractValidator<UpdateDeliverableCommand>
{
    public UpdateDeliverableCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(250).WithMessage("Title cannot exceed 250 characters.");

        RuleFor(x => x.Platform)
            .NotEmpty().WithMessage("Platform is required.")
            .IsEnumName(typeof(PlatformType), caseSensitive: false)
            .WithMessage("Platform is invalid. Allowed: Instagram, TikTok, YouTube, TwitterX, Other.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("ContentType is required.")
            .IsEnumName(typeof(ContentType), caseSensitive: false)
            .WithMessage("ContentType is invalid. Allowed: Reel, Story, FeedPost, Shorts, DedicatedVideo, IntegratedVideo.");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("DueDate is required.");
    }
}

