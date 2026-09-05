namespace CampaignSaaS.Modules.Client.Application.Commands.UpdateClient;

using FluentValidation;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Client ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(200).WithMessage("Client name must not exceed 200 characters.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.");

        RuleForEach(x => x.Contacts).ChildRules(contact =>
        {
            contact.RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Contact name is required.")
                .MaximumLength(150).WithMessage("Contact name must not exceed 150 characters.");

            contact.RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("A valid contact email is required.")
                .MaximumLength(255).WithMessage("Contact email must not exceed 255 characters.");
        });
    }
}
