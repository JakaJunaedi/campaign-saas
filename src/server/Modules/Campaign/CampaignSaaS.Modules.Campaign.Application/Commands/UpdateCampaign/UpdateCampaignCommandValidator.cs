namespace CampaignSaaS.Modules.Campaign.Application.Commands.UpdateCampaign;

using FluentValidation;

public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Campaign ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Campaign title is required.")
            .MaximumLength(250).WithMessage("Campaign title must not exceed 250 characters.");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget cannot be negative.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than or equal to start date.");
    }
}
