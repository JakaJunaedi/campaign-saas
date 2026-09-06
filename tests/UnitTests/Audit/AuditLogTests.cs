namespace CampaignSaaS.UnitTests.Audit;

using CampaignSaaS.Modules.Audit.Domain.Entities;
using FluentAssertions;
using Xunit;

public class AuditLogTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAuditLog()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = "admin@example.com";
        var action = "UpdateCampaign";
        var module = "Campaign";
        var entityName = "Campaign";
        var entityId = Guid.NewGuid();
        var changesJson = "{\"status\":\"Active\"}";
        var ip = "127.0.0.1";

        // Act
        var log = AuditLog.Create(orgId, userId, email, action, module, entityName, entityId, changesJson, ip);

        // Assert
        log.Should().NotBeNull();
        log.Id.Should().NotBeEmpty();
        log.OrganizationId.Should().Be(orgId);
        log.UserId.Should().Be(userId);
        log.ActorEmail.Should().Be(email);
        log.Action.Should().Be(action);
        log.Module.Should().Be(module);
        log.EntityName.Should().Be(entityName);
        log.EntityId.Should().Be(entityId);
        log.ChangesJson.Should().Be(changesJson);
        log.IpAddress.Should().Be(ip);
        log.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("", "Campaign", "Campaign")]
    [InlineData("Action", "", "Campaign")]
    [InlineData("Action", "Campaign", "")]
    public void Create_WithInvalidParameters_ShouldThrowArgumentException(string action, string module, string entityName)
    {
        // Act
        var act = () => AuditLog.Create(Guid.NewGuid(), null, null, action, module, entityName, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
