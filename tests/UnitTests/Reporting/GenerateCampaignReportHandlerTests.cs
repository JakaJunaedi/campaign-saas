namespace CampaignSaaS.UnitTests.Reporting;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.Modules.Reporting.Domain.Enums;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using MassTransit;
using NSubstitute;
using Xunit;

public class GenerateCampaignReportHandlerTests
{
    private readonly ICampaignReportRepository _reportRepo = Substitute.For<ICampaignReportRepository>();
    private readonly IReportingUnitOfWork _uow = Substitute.For<IReportingUnitOfWork>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly GenerateCampaignReportCommandHandler _sut;

    public GenerateCampaignReportHandlerTests()
    {
        _sut = new GenerateCampaignReportCommandHandler(
            _reportRepo,
            _uow,
            _publishEndpoint,
            _tenantContext);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateReportWithPendingStatusAndPublishJobToOutbox()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var campaignId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);

        var command = new GenerateCampaignReportCommand(campaignId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.CampaignId.Should().Be(campaignId);
        result.Value.OrganizationId.Should().Be(orgId);
        result.Value.Status.Should().Be(ReportStatus.Pending.ToString());
        result.Value.FileObjectKey.Should().BeNull();
        result.Value.DownloadUrl.Should().BeNull();

        await _reportRepo.Received(1).AddAsync(Arg.Any<CampaignReport>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<GenerateCampaignReportJob>(j => j.CampaignId == campaignId && j.OrganizationId == orgId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoActiveTenant_ShouldReturnForbiddenError()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new GenerateCampaignReportCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }
}
