namespace CampaignSaaS.Modules.Audit.Application.Commands.RecordAuditLog;

using CampaignSaaS.Modules.Audit.Application.Abstractions;
using CampaignSaaS.Modules.Audit.Contracts.DTOs;
using CampaignSaaS.Modules.Audit.Domain.Entities;
using ErrorOr;
using MediatR;

public class RecordAuditLogCommandHandler : IRequestHandler<RecordAuditLogCommand, ErrorOr<AuditLogDto>>
{
    private readonly IAuditLogRepository _repository;
    private readonly IAuditUnitOfWork _unitOfWork;

    public RecordAuditLogCommandHandler(
        IAuditLogRepository repository,
        IAuditUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<AuditLogDto>> Handle(
        RecordAuditLogCommand request,
        CancellationToken cancellationToken)
    {
        var auditLog = AuditLog.Create(
            request.OrganizationId,
            request.UserId,
            request.ActorEmail,
            request.Action,
            request.Module,
            request.EntityName,
            request.EntityId,
            request.ChangesJson,
            request.IpAddress);

        await _repository.AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuditLogDto(
            auditLog.Id,
            auditLog.OrganizationId,
            auditLog.UserId,
            auditLog.ActorEmail,
            auditLog.Action,
            auditLog.Module,
            auditLog.EntityName,
            auditLog.EntityId,
            auditLog.ChangesJson,
            auditLog.IpAddress,
            auditLog.Timestamp);
    }
}
