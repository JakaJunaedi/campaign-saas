# Domain Model Specification — Campaign SaaS

## 1. Document Control
- **Title**: Domain Model & Ubiquitous Language Specification
- **Purpose**: Mendefinisikan agregat, entitas, value object, dan domain events untuk setiap modul.
- **Status**: ACCEPTED
- **Scope**: Core Domain Architecture

---

## 2. Core Aggregates per Module

```mermaid
classDiagram
    class Organization {
        +Guid Id
        +string Name
        +string Slug
        +OrganizationStatus Status
    }

    class User {
        +Guid Id
        +Guid OrganizationId
        +string Email
        +string FullName
        +UserRole Role
        +bool IsActive
    }

    class Client {
        +Guid Id
        +Guid OrganizationId
        +string Name
        +string CompanyName
        +List~ClientContact~ Contacts
    }

    class Campaign {
        +Guid Id
        +Guid OrganizationId
        +Guid ClientId
        +string Title
        +decimal Budget
        +DateOnly StartDate
        +DateOnly EndDate
        +CampaignStatus Status
    }

    class Creator {
        +Guid Id
        +Guid OrganizationId
        +string FullName
        +string Email
        +string PhoneNumber
        +string Niche
        +List~SocialAccount~ SocialAccounts
    }

    class CampaignCreator {
        +Guid Id
        +Guid OrganizationId
        +Guid CampaignId
        +Guid CreatorId
        +CampaignCreatorStatus Status
        +decimal AgreedRate
    }

    class Deliverable {
        +Guid Id
        +Guid OrganizationId
        +Guid CampaignId
        +Guid CampaignCreatorId
        +string Title
        +PlatformType Platform
        +ContentType ContentType
        +DeliverableStatus Status
        +DateOnly DueDate
        +string LiveUrl
    }

    class ContentSubmission {
        +Guid Id
        +Guid DeliverableId
        +int VersionNumber
        +string MediaObjectKey
        +string Caption
        +DateTime SubmittedAt
    }

    class ApprovalReview {
        +Guid Id
        +Guid ContentSubmissionId
        +Guid ReviewerId
        +ReviewDecision Decision
        +string FeedbackNotes
        +DateTime ReviewedAt
    }

    class CampaignMetric {
        +Guid Id
        +Guid OrganizationId
        +Guid DeliverableId
        +long Reach
        +long Impressions
        +long Views
        +long Likes
        +long Comments
        +long Shares
        +long Clicks
        +long TotalEngagement
    }

    Organization "1" --> "*" User
    Organization "1" --> "*" Client
    Organization "1" --> "*" Campaign
    Organization "1" --> "*" Creator
    Campaign "1" --> "*" CampaignCreator
    CampaignCreator "1" --> "*" Deliverable
    Deliverable "1" --> "*" ContentSubmission
    ContentSubmission "1" --> "*" ApprovalReview
    Deliverable "1" --> "1" CampaignMetric
```

---

## 3. Domain Enums & Value Objects

### 3.1 Enums
- **`OrganizationStatus`**: `Active`, `Suspended`, `Trial`
- **`UserRole`**: `AgencyOwner`, `CampaignManager`, `ContentReviewer`, `Creator`
- **`CampaignStatus`**: `Draft`, `Planned`, `Active`, `Paused`, `Completed`, `Cancelled`
- **`CampaignCreatorStatus`**: `Shortlisted`, `Invited`, `Accepted`, `Rejected`, `Negotiation`, `Confirmed`, `Completed`
- **`DeliverableStatus`**: `Pending`, `Submitted`, `Revision`, `Approved`, `Published`, `Rejected`
- **`ReviewDecision`**: `Approved`, `RevisionRequested`, `Rejected`
- **`PlatformType`**: `Instagram`, `TikTok`, `YouTube`, `TwitterX`, `Other`
- **`ContentType`**: `Reel`, `Story`, `FeedPost`, `Shorts`, `DedicatedVideo`, `IntegratedVideo`

### 3.2 Value Objects
- **`SocialAccount`**: `PlatformType Platform`, `string Handle`, `string ProfileUrl`, `long FollowerCount`
- **`Money`**: `decimal Amount`, `string Currency (default: IDR)`
- **`ClientContact`**: `string Name`, `string Email`, `string PhoneNumber`, `string Position`

---

## 4. Domain Events
- **`CampaignCreatedEvent`**: Dipicu saat campaign baru dibuat.
- **`CampaignStatusChangedEvent`**: Dipicu saat status campaign berubah.
- **`CreatorInvitedEvent`**: Dipicu saat creator diundang ke roster campaign.
- **`DeliverableSubmittedEvent`**: Dipicu saat creator mengunggah submission konten baru.
- **`RevisionRequestedEvent`**: Dipicu saat reviewer meminta revisi materi konten.
- **`ContentApprovedEvent`**: Dipicu saat draft konten disetujui.
- **`ContentPublishedEvent`**: Dipicu saat link posting live diunggah.
- **`ReportGeneratedEvent`**: Dipicu saat file PDF laporan berhasil diproses jsreport.
