namespace CampaignSaaS.UnitTests;

using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using Xunit;

public class TestEntity : Entity<Guid>
{
    public TestEntity(Guid id) : base(id) { }
}

public class SharedKernelTests
{
    [Fact]
    public void Entities_WithSameId_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        (entity1 == entity2).Should().BeTrue();
        entity1.Equals(entity2).Should().BeTrue();
    }

    [Fact]
    public void CurrentTenantContext_ShouldStoreTenantIdCorrectly()
    {
        var tenantContext = new CurrentTenantContext();
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        tenantContext.SetTenant(orgId, userId, isSuperAdmin: false);

        tenantContext.OrganizationId.Should().Be(orgId);
        tenantContext.CurrentUserId.Should().Be(userId);
        tenantContext.IsSuperAdmin.Should().BeFalse();
    }
}
