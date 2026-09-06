namespace CampaignSaaS.UnitTests.Deliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Application.Commands.CreateDeliverable;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateDeliverableHandlerTests
{
    private readonly IDeliverableRepository _deliverableRepo = Substitute.For<IDeliverableRepository>();
    private readonly IDeliverableUnitOfWork _uow = Substitute.For<IDeliverableUnitOfWork>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly CreateDeliverableCommandHandler _sut;

    public CreateDeliverableHandlerTests()
    {
        _sut = new CreateDeliverableCommandHandler(_deliverableRepo, _uow, _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateDeliverable()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        var rosterId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var command = new CreateDeliverableCommand(
            campaignId,
            rosterId,
            "Product Unboxing Reel",
            "Instagram",
            "Reel",
            "Show packaging and mention key USP",
            new DateOnly(2026, 6, 15));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Title.Should().Be("Product Unboxing Reel");
        result.Value.Platform.Should().Be("Instagram");
        result.Value.ContentType.Should().Be("Reel");
        result.Value.Status.Should().Be(DeliverableStatus.Pending.ToString());

        await _deliverableRepo.Received(1).AddAsync(Arg.Is<Deliverable>(d => d.Title == "Product Unboxing Reel" && d.OrganizationId == orgId), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidPlatform_ShouldReturnValidationError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns(Guid.NewGuid());
        var command = new CreateDeliverableCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", "InvalidPlatform", "Reel", null, new DateOnly(2026, 6, 15));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Deliverable.InvalidPlatform");
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new CreateDeliverableCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", "Instagram", "Reel", null, new DateOnly(2026, 6, 15));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}

