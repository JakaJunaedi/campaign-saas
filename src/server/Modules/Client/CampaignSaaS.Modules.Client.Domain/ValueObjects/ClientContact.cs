namespace CampaignSaaS.Modules.Client.Domain.ValueObjects;

using CampaignSaaS.SharedKernel.Domain;

public class ClientContact : ValueObject
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Position { get; private set; }

    private ClientContact() { }

    public ClientContact(string name, string email, string? phoneNumber = null, string? position = null)
    {
        Name = name.Trim();
        Email = email.ToLowerInvariant().Trim();
        PhoneNumber = phoneNumber?.Trim();
        Position = position?.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Email;
        yield return PhoneNumber;
        yield return Position;
    }
}
