namespace CampaignSaaS.Modules.Audit.Application.Commands.RecordAuditLog;

using CampaignSaaS.Modules.Audit.Contracts.DTOs;
using ErrorOr;
using MediatR;

public record RecordAuditLogCommand(
    Guid OrganizationId,
    Guid? UserId,
    string? ActorEmail,
    string Action,
    string Module,
    string EntityName,
    Guid EntityId,
    string? ChangesJson = null,
    string? IpAddress = null
) : IRequest<ErrorOr<AuditLogDto>>;
