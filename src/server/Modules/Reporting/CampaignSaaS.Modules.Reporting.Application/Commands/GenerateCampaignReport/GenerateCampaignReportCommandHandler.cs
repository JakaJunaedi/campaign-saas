namespace CampaignSaaS.Modules.Reporting.Application.Commands.GenerateCampaignReport;

using CampaignSaaS.Modules.Reporting.Application.Abstractions;
using CampaignSaaS.Modules.Reporting.Contracts.DTOs;
using CampaignSaaS.Modules.Reporting.Domain.Entities;
using CampaignSaaS.SharedKernel.IntegrationEvents;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MassTransit;
using MediatR;

public class GenerateCampaignReportCommandHandler : IRequestHandler<GenerateCampaignReportCommand, ErrorOr<CampaignReportDto>>
{
    private readonly ICampaignReportRepository _reportRepository;
    private readonly IReportingUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICurrentTenantContext _tenantContext;

    public GenerateCampaignReportCommandHandler(
        ICampaignReportRepository reportRepository,
        IReportingUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ICurrentTenantContext tenantContext)
    {
        _reportRepository = reportRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<CampaignReportDto>> Handle(GenerateCampaignReportCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var report = CampaignReport.RequestReport(orgId, request.CampaignId);
        await _reportRepository.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new GenerateCampaignReportJob(
            report.Id,
            orgId,
            request.CampaignId,
            DateTime.UtcNow), cancellationToken);

        return new CampaignReportDto(
            report.Id,
            report.OrganizationId,
            report.CampaignId,
            report.Status.ToString(),
            report.FileObjectKey,
            null,
            report.ErrorMessage,
            report.RequestedAt,
            report.CompletedAt);
    }
}
