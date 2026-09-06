namespace CampaignSaaS.Modules.Deliverable.Application.Commands.CreateDeliverable;

using CampaignSaaS.Modules.Deliverable.Application.Abstractions;
using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.Modules.Deliverable.Domain.Entities;
using CampaignSaaS.Modules.Deliverable.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class CreateDeliverableCommandHandler : IRequestHandler<CreateDeliverableCommand, ErrorOr<DeliverableDto>>
{
    private readonly IDeliverableRepository _deliverableRepository;
    private readonly IDeliverableUnitOfWork _unitOfWork;
    private readonly ICurrentTenantContext _tenantContext;

    public CreateDeliverableCommandHandler(
        IDeliverableRepository deliverableRepository,
        IDeliverableUnitOfWork unitOfWork,
        ICurrentTenantContext tenantContext)
    {
        _deliverableRepository = deliverableRepository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ErrorOr<DeliverableDto>> Handle(CreateDeliverableCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        if (!Enum.TryParse<PlatformType>(request.Platform, true, out var platform))
        {
            return Error.Validation("Deliverable.InvalidPlatform", $"Platform '{request.Platform}' is invalid.");
        }

        if (!Enum.TryParse<ContentType>(request.ContentType, true, out var contentType))
        {
            return Error.Validation("Deliverable.InvalidContentType", $"ContentType '{request.ContentType}' is invalid.");
        }

        var deliverable = Deliverable.Create(
            orgId,
            request.CampaignId,
            request.CampaignCreatorId,
            request.Title,
            platform,
            contentType,
            request.BriefNotes,
            request.DueDate);

        await _deliverableRepository.AddAsync(deliverable, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeliverableDto(
            deliverable.Id,
            deliverable.OrganizationId,
            deliverable.CampaignId,
            deliverable.CampaignCreatorId,
            deliverable.Title,
            deliverable.Platform.ToString(),
            deliverable.ContentType.ToString(),
            deliverable.BriefNotes,
            deliverable.DueDate,
            deliverable.PostingDate,
            deliverable.Status.ToString(),
            deliverable.LiveUrl,
            deliverable.ProofMediaKey,
            0,
            deliverable.CreatedAt,
            deliverable.UpdatedAt);
    }
}

