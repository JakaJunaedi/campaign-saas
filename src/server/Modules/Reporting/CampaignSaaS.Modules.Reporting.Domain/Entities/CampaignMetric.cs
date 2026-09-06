namespace CampaignSaaS.Modules.Reporting.Domain.Entities;

using CampaignSaaS.Modules.Reporting.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class CampaignMetric : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid DeliverableId { get; private set; }
    public long Reach { get; private set; }
    public long Impressions { get; private set; }
    public long Views { get; private set; }
    public long Likes { get; private set; }
    public long Comments { get; private set; }
    public long Shares { get; private set; }
    public long Clicks { get; private set; }
    public long TotalEngagement { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; } = DateTimeOffset.UtcNow;

    private CampaignMetric() { }

    private CampaignMetric(
        Guid id,
        Guid organizationId,
        Guid deliverableId,
        long reach,
        long impressions,
        long views,
        long likes,
        long comments,
        long shares,
        long clicks) : base(id)
    {
        OrganizationId = organizationId;
        DeliverableId = deliverableId;
        Reach = Math.Max(0, reach);
        Impressions = Math.Max(0, impressions);
        Views = Math.Max(0, views);
        Likes = Math.Max(0, likes);
        Comments = Math.Max(0, comments);
        Shares = Math.Max(0, shares);
        Clicks = Math.Max(0, clicks);
        TotalEngagement = Likes + Comments + Shares + Clicks;
        RecordedAt = DateTimeOffset.UtcNow;
    }

    public static CampaignMetric Record(
        Guid organizationId,
        Guid deliverableId,
        long reach,
        long impressions,
        long views,
        long likes,
        long comments,
        long shares,
        long clicks)
    {
        var metric = new CampaignMetric(
            Guid.NewGuid(),
            organizationId,
            deliverableId,
            reach,
            impressions,
            views,
            likes,
            comments,
            shares,
            clicks);

        metric.AddDomainEvent(new MetricsRecordedDomainEvent(
            metric.Id,
            metric.OrganizationId,
            metric.DeliverableId,
            metric.Reach,
            metric.Impressions,
            metric.Views,
            metric.TotalEngagement,
            metric.RecordedAt));

        return metric;
    }

    public void UpdateMetrics(
        long reach,
        long impressions,
        long views,
        long likes,
        long comments,
        long shares,
        long clicks)
    {
        Reach = Math.Max(0, reach);
        Impressions = Math.Max(0, impressions);
        Views = Math.Max(0, views);
        Likes = Math.Max(0, likes);
        Comments = Math.Max(0, comments);
        Shares = Math.Max(0, shares);
        Clicks = Math.Max(0, clicks);
        TotalEngagement = Likes + Comments + Shares + Clicks;
        RecordedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new MetricsRecordedDomainEvent(
            Id,
            OrganizationId,
            DeliverableId,
            Reach,
            Impressions,
            Views,
            TotalEngagement,
            RecordedAt));
    }

    public double CalculateEngagementRate()
    {
        if (Reach > 0)
        {
            return Math.Round((double)TotalEngagement / Reach * 100.0, 2);
        }
        if (Impressions > 0)
        {
            return Math.Round((double)TotalEngagement / Impressions * 100.0, 2);
        }
        return 0.0;
    }
}
