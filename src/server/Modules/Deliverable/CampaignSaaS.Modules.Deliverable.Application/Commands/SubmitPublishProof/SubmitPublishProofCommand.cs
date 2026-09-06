namespace CampaignSaaS.Modules.Deliverable.Application.Commands.SubmitPublishProof;

using CampaignSaaS.Modules.Deliverable.Contracts.DTOs;
using CampaignSaaS.SharedKernel.Application;

public record SubmitPublishProofCommand(
    Guid DeliverableId,
    string LiveUrl,
    string? ProofMediaKey,
    DateOnly? PostingDate) : ICommand<DeliverableDto>;

