# UI Screen Map & Route Hierarchy — Campaign SaaS

## 1. Document Control
- **Title**: UI Screen Map & Navigation Structure
- **Purpose**: Struktur navigasi dan pemetaan layar frontend Angular 21.
- **Status**: ACCEPTED
- **Scope**: Frontend Route Structure

---

## 2. Route Hierarchy (Angular 21 Standalone)

```text
/ (Root Redirect)
├── /auth
│   ├── /login
│   ├── /forgot-password
│   └── /reset-password
│

├── /admin (SuperAdmin Console - Platform Management)
│   ├── /overview                            # Global Platform Metrics (Total Tenants, Storage, Active Campaigns)
│   ├── /organizations                       # Organizations Master Directory (Create, Suspend, Quota Limits)
│   │   └── /:orgId                          # Organization Deep Dive, Usage Breakdown, Impersonation Entry
│   ├── /users                               # Global Users Directory
│   ├── /system-health                       # Container Status, Queue Lag, Storage Usage
│   └── /global-audit-logs                   # Cross-Tenant Security Audit Trails
│
├── /app (Agency Portal - Layout with Sidebar & Header)
│   ├── /dashboard                           # Ringkasan KPI, Action Items, Active Campaigns
│   ├── /clients                             # Client List Table
│   │   ├── /new                             # Create Client Modal/Page
│   │   └── /:clientId                       # Client Detail, Contacts & Campaigns
│   ├── /campaigns                           # Campaign Directory
│   │   ├── /new                             # Step-by-step Campaign Wizard
│   │   └── /:campaignId
│   │       ├── /overview                    # Campaign Summary, Dates, Budget
│   │       ├── /roster                      # Creator Roster (Kanban / Table)
│   │       ├── /deliverables                # Master Deliverables Table
│   │       │   └── /:deliverableId          # Review Canvas, Media Player, Comments
│   │       └── /reporting                   # Metrics Input & PDF Export View
│   ├── /creators                            # Internal Creator CRM Database
│   │   ├── /new                             # Add Creator Form
│   │   └── /:creatorId                      # Creator Profile, Socials, Past Campaigns
│   ├── /settings
│   │   ├── /organization                    # Organization Profile
│   │   ├── /team                            # Users & RBAC Roles
│   │   └── /audit-logs                      # Audit Trail History
│   └── /notifications                       # In-App Notification Center
│
└── /portal (Creator Portal - Mobile-Optimized Layout)
    ├── /my-campaigns                        # List of Assigned Campaigns
    └── /deliverables/:deliverableId         # Brief View, Upload Draft, Live Link Submission
```
