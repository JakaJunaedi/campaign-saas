# Database Design & Schema — Campaign SaaS

## 1. Document Control
- **Title**: Database Design & Relational Schema
- **Purpose**: Spesifikasi skema tabel PostgreSQL, relasi foreign key, indeks performa, dan aturan multi-tenant isolation.
- **Status**: ACCEPTED
- **Database Engine**: PostgreSQL 16+ (Managed / Docker Container)

---

## 2. Tabel & Relasi PostgreSQL

```sql
-- 1. Identity Module
CREATE TABLE organizations (
    id UUID PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    slug VARCHAR(100) UNIQUE NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Active',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE users (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    full_name VARCHAR(150) NOT NULL,
    role VARCHAR(50) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    CONSTRAINT uq_users_org_email UNIQUE (organization_id, email)
);
CREATE INDEX idx_users_org ON users(organization_id);

-- 2. Client Module
CREATE TABLE clients (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    company_name VARCHAR(200) NULL,
    contacts_json JSONB NOT NULL DEFAULT '[]',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE INDEX idx_clients_org ON clients(organization_id);

-- 3. Campaign Module
CREATE TABLE campaigns (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    client_id UUID NOT NULL, -- Logical reference to clients(id)
    title VARCHAR(250) NOT NULL,
    description TEXT NULL,
    budget NUMERIC(15, 2) NOT NULL DEFAULT 0,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Draft',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE INDEX idx_campaigns_org_status ON campaigns(organization_id, status);

CREATE TABLE campaign_creators (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    campaign_id UUID NOT NULL REFERENCES campaigns(id) ON DELETE CASCADE,
    creator_id UUID NOT NULL, -- Logical reference to creators(id)
    status VARCHAR(50) NOT NULL DEFAULT 'Shortlisted',
    agreed_rate NUMERIC(15, 2) NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    CONSTRAINT uq_campaign_creator UNIQUE (campaign_id, creator_id)
);
CREATE INDEX idx_campaign_creators_org ON campaign_creators(organization_id);

-- 4. Creator Module
CREATE TABLE creators (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    full_name VARCHAR(200) NOT NULL,
    email VARCHAR(255) NULL,
    phone_number VARCHAR(50) NULL,
    niche VARCHAR(100) NOT NULL,
    social_accounts_json JSONB NOT NULL DEFAULT '[]',
    status VARCHAR(50) NOT NULL DEFAULT 'Active',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE INDEX idx_creators_org ON creators(organization_id);

-- 5. Deliverable Module
CREATE TABLE deliverables (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    campaign_id UUID NOT NULL, -- Logical reference to campaigns(id)
    campaign_creator_id UUID NOT NULL, -- Logical reference to campaign_creators(id)
    title VARCHAR(250) NOT NULL,
    platform VARCHAR(50) NOT NULL,
    content_type VARCHAR(50) NOT NULL,
    brief_notes TEXT NULL,
    due_date DATE NOT NULL,
    posting_date DATE NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    live_url TEXT NULL,
    proof_media_key VARCHAR(500) NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL
);
CREATE INDEX idx_deliverables_org_status ON deliverables(organization_id, status);
CREATE INDEX idx_deliverables_campaign ON deliverables(campaign_id);

-- 6. Approval Module
CREATE TABLE content_submissions (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    deliverable_id UUID NOT NULL, -- Logical reference to deliverables(id)
    version_number INT NOT NULL DEFAULT 1,
    media_object_key VARCHAR(500) NOT NULL,
    media_file_name VARCHAR(255) NOT NULL,
    media_file_size BIGINT NOT NULL,
    caption TEXT NULL,
    submitted_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_submission_version UNIQUE (deliverable_id, version_number)
);
CREATE INDEX idx_submissions_deliverable ON content_submissions(deliverable_id);

CREATE TABLE approval_reviews (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    content_submission_id UUID NOT NULL REFERENCES content_submissions(id) ON DELETE CASCADE,
    reviewer_id UUID NOT NULL, -- Logical reference to users(id)
    decision VARCHAR(50) NOT NULL,
    feedback_notes TEXT NULL,
    reviewed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_reviews_submission ON approval_reviews(content_submission_id);

-- 7. Reporting Module
CREATE TABLE campaign_metrics (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    deliverable_id UUID NOT NULL, -- Logical reference to deliverables(id)
    reach BIGINT NOT NULL DEFAULT 0,
    impressions BIGINT NOT NULL DEFAULT 0,
    views BIGINT NOT NULL DEFAULT 0,
    likes BIGINT NOT NULL DEFAULT 0,
    comments BIGINT NOT NULL DEFAULT 0,
    shares BIGINT NOT NULL DEFAULT 0,
    clicks BIGINT NOT NULL DEFAULT 0,
    total_engagement BIGINT NOT NULL DEFAULT 0,
    recorded_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_metrics_deliverable UNIQUE (deliverable_id)
);
CREATE INDEX idx_metrics_org ON campaign_metrics(organization_id);

CREATE TABLE campaign_reports (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    campaign_id UUID NOT NULL, -- Logical reference to campaigns(id)
    title VARCHAR(250) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    pdf_object_key VARCHAR(500) NULL,
    generated_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_reports_campaign ON campaign_reports(campaign_id);

-- 8. Notification Module
CREATE TABLE notifications (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    user_id UUID NOT NULL, -- Logical reference to users(id)
    title VARCHAR(200) NOT NULL,
    message TEXT NOT NULL,
    type VARCHAR(50) NOT NULL,
    is_read BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_notifications_user ON notifications(user_id, is_read);

-- 9. Audit Module
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL,
    user_id UUID NULL,
    entity_name VARCHAR(100) NOT NULL,
    entity_id UUID NOT NULL,
    action VARCHAR(50) NOT NULL,
    changes_json JSONB NULL,
    ip_address VARCHAR(45) NULL,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_audit_org_timestamp ON audit_logs(organization_id, timestamp DESC);

-- 10. Messaging Infrastructure (Transactional Outbox)
-- outbox_messages & inbox_messages dikelola oleh MassTransit EF Core Outbox state.
```

---

## 3. Modular Monolith & Multi-Tenancy Boundary Rules
1. **No Cross-Module Database Foreign Keys**: Sesuai *Strict Module Rule 1 & Rule 2*, referensi lintas modul menggunakan tipe data primitif `Guid` (`organization_id`, `client_id`, `creator_id`, `deliverable_id`, dsb.). Tidak ada foreign key constraint antar schema modul yang berbeda untuk menjamin otonomi modul dan kemudahan migrasi ke microservices.
2. **Organization Boundary Enforcement**: Setiap entitas bisnis wajib memiliki kolom `organization_id`.
3. **EF Core 10 Global Query Filter**:
   ```csharp
   modelBuilder.Entity<T>().HasQueryFilter(e => e.OrganizationId == _tenantContext.OrganizationId && !e.IsDeleted);
   ```

