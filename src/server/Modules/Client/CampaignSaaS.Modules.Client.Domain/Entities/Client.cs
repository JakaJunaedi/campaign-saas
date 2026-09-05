namespace CampaignSaaS.Modules.Client.Domain.Entities;

using CampaignSaaS.Modules.Client.Domain.Events;
using CampaignSaaS.Modules.Client.Domain.ValueObjects;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class Client : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? CompanyName { get; private set; }
    private readonly List<ClientContact> _contacts = new();
    public IReadOnlyList<ClientContact> Contacts => _contacts.AsReadOnly();
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Client() { }

    private Client(Guid id, Guid organizationId, string name, string? companyName)
        : base(id)
    {
        OrganizationId = organizationId;
        Name = name.Trim();
        CompanyName = companyName?.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
        IsDeleted = false;
    }

    public static Client Create(
        Guid organizationId,
        string name,
        string? companyName = null,
        IEnumerable<ClientContact>? contacts = null)
    {
        var client = new Client(Guid.NewGuid(), organizationId, name, companyName);
        if (contacts != null)
        {
            client._contacts.AddRange(contacts);
        }

        client.AddDomainEvent(new ClientCreatedDomainEvent(client.Id, client.OrganizationId, client.Name, client.CreatedAt));
        return client;
    }

    public void Update(string name, string? companyName)
    {
        Name = name.Trim();
        CompanyName = companyName?.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new ClientUpdatedDomainEvent(Id, OrganizationId, Name, UpdatedAt.Value));
    }

    public void SetContacts(IEnumerable<ClientContact> contacts)
    {
        _contacts.Clear();
        _contacts.AddRange(contacts);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddContact(ClientContact contact)
    {
        _contacts.Add(contact);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveContact(string email)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();
        _contacts.RemoveAll(c => string.Equals(c.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase));
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new ClientDeletedDomainEvent(Id, OrganizationId, UpdatedAt.Value));
    }
}
