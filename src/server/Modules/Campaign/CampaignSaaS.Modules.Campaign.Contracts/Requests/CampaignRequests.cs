namespace CampaignSaaS.Modules.Campaign.Contracts.Requests;

public record CreateCampaignRequest(
    Guid ClientId,
    string Title,
    string? Description,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate);

public record UpdateCampaignRequest(
    string Title,
    string? Description,
    decimal Budget,
    DateOnly StartDate,
    DateOnly EndDate);

public record UpdateCampaignStatusRequest(
    string Status);

public record AddCreatorToRosterRequest(
    Guid CreatorId,
    decimal AgreedRate = 0);

public record UpdateRosterStatusRequest(
    string Status,
    decimal? AgreedRate = null);
