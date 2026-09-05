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
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
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
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    client_id UUID NOT NULL REFERENCES clients(id) ON DELETE RESTRICT,
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

-- 4. Creator Module
CREATE TABLE creators (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
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

-- 5. Campaign Creator (Roster)
CREATE TABLE campaign_creators (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    campaign_id UUID NOT NULL REFERENCES campaigns(id) ON DELETE CASCADE,
    creator_id UUID NOT NULL REFERENCES creators(id) ON DELETE RESTRICT,
    status VARCHAR(50) NOT NULL DEFAULT 'Shortlisted',
    agreed_rate NUMERIC(15, 2) NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NULL,
    CONSTRAINT uq_campaign_creator UNIQUE (campaign_id, creator_id)
);
CREATE INDEX idx_campaign_creators_org ON campaign_creators(organization_id);

-- 6. Deliverable Module
CREATE TABLE deliverables (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    campaign_id UUID NOT NULL REFERENCES campaigns(id) ON DELETE CASCADE,
    campaign_creator_id UUID NOT NULL REFERENCES campaign_creators(id) ON DELETE CASCADE,
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

-- 7. Content Submissions (Versioning)
CREATE TABLE content_submissions (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    deliverable_id UUID NOT NULL REFERENCES deliverables(id) ON DELETE CASCADE,
    version_number INT NOT NULL DEFAULT 1,
    media_object_key VARCHAR(500) NOT NULL,
    media_file_name VARCHAR(255) NOT NULL,
    media_file_size BIGINT NOT NULL,
    caption TEXT NULL,
    submitted_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_submission_version UNIQUE (deliverable_id, version_number)
);
CREATE INDEX idx_submissions_deliverable ON content_submissions(deliverable_id);

-- 8. Approval Reviews & Comments
CREATE TABLE approval_reviews (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    content_submission_id UUID NOT NULL REFERENCES content_submissions(id) ON DELETE CASCADE,
    reviewer_id UUID NOT NULL REFERENCES users(id),
    decision VARCHAR(50) NOT NULL,
    feedback_notes TEXT NULL,
    reviewed_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_reviews_submission ON approval_reviews(content_submission_id);

-- 9. Reporting & Metrics
CREATE TABLE campaign_metrics (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    deliverable_id UUID NOT NULL REFERENCES deliverables(id) ON DELETE CASCADE,
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

-- 10. Audit Logs
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY,
    organization_id UUID NOT NULL REFERENCES organizations(id) ON DELETE CASCADE,
    user_id UUID NULL,
    entity_name VARCHAR(100) NOT NULL,
    entity_id UUID NOT NULL,
    action VARCHAR(50) NOT NULL,
    changes_json JSONB NULL,
    ip_address VARCHAR(45) NULL,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_audit_org_timestamp ON audit_logs(organization_id, timestamp DESC);
```

---

## 3. Aturan Multi-Tenancy di Tingkat Database
- Setiap tabel bisnis WAJIB memiliki kolom `organization_id`.
- EF Core 10 dikonfigurasi dengan **Global Query Filter**:
  ```csharp
  modelBuilder.Entity<Campaign>().HasQueryFilter(e => e.OrganizationId == _tenantContext.OrganizationId && !e.IsDeleted);
  ```
