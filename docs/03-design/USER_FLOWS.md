# User Flows & Workflow Diagrams — Campaign SaaS

## 1. Document Control
- **Title**: User Flows & Interaction Specification
- **Purpose**: Memetakan seluruh alur pengguna interaktif dari inisiasi campaign hingga final reporting.
- **Status**: ACCEPTED
- **Scope**: Core MVP Flows

---

## 2. End-to-End Campaign Lifecycle Flow

```mermaid
sequenceDiagram
    autonumber
    actor CM as Campaign Manager
    actor CTR as Creator
    actor REV as Content Reviewer
    participant SYS as Campaign SaaS App
    participant WF as Camunda BPMN Engine
    participant S3 as MinIO Storage
    participant REP as jsreport Service

    CM->>SYS: 1. Buat Campaign & Assign Client
    CM->>SYS: 2. Pilih Creator & Masukkan ke Roster (Status: SHORTLISTED -> INVITED -> CONFIRMED)
    CM->>SYS: 3. Tentukan Deliverable (Platform, Type, Deadlines)
    SYS->>WF: 4. Start Deliverable Process Instance (Status: PENDING)
    
    CTR->>SYS: 5. Buka Deliverable & Request Presigned Upload URL
    SYS->>S3: 6. Upload Media File (Video/Photo/Doc)
    CTR->>SYS: 7. Submit Content Draft (Media Key, Caption)
    SYS->>WF: 8. Trigger Event: ContentSubmitted (Status: SUBMITTED)

    REV->>SYS: 9. Review Media Draft & Caption
    alt Minta Revisi
        REV->>SYS: 10a. Request Revision with Comments
        SYS->>WF: 11a. Trigger Event: RevisionRequested (Status: REVISION)
        CTR->>SYS: 12a. Upload Resubmission (Version Increment)
        SYS->>WF: 13a. Resubmitted (Status: SUBMITTED)
    else Approve Konten
        REV->>SYS: 10b. Approve Content
        SYS->>WF: 11b. Trigger Event: ContentApproved (Status: APPROVED)
    end

    CTR->>SYS: 14. Post ke Media Sosial & Submit Live URL + Proof Screenshot
    SYS->>WF: 15. Trigger Event: ContentPublished (Status: PUBLISHED)

    CM->>SYS: 16. Input Manual Metrics (Reach, Views, Engagement)
    CM->>SYS: 17. Request Export PDF Report
    SYS->>REP: 18. Generate PDF via RabbitMQ Worker
    REP-->>CM: 19. Download Completed Campaign Report PDF
```

---

## 3. Detail Sub-Flows

### 3.1 Flow Creator Assignment & Status Transition
```text
[SHORTLISTED] ──(Kirim Undangan)──> [INVITED]
   │                                   │
   └───(Ditolak Agensi/KOL)──────┐    ├──(KOL Menolak)──> [REJECTED]
                                 │    └──(KOL Setuju)───> [ACCEPTED]
                                 │                           │
                                 │                (Diskusi Rate/Fee)
                                 │                           │
                                 │                           ▼
                                 │                     [NEGOTIATION]
                                 │                           │
                                 │                    (Kesepakatan)
                                 │                           │
                                 │                           ▼
                                 │                     [CONFIRMED]
                                 │                           │
                                 │                    (Campaign Selesai)
                                 │                           │
                                 ▼                           ▼
                            [CANCELLED]                [COMPLETED]
```

### 3.2 Flow Deliverable & Content Approval
```text
[PENDING] ──(Creator Upload Draft)──> [SUBMITTED]
                                           │
                                    (Reviewer Evaluasi)
                                           │
                    ┌──────────────────────┴──────────────────────┐
                    │                                             │
             (Butuh Perbaikan)                                (Sesuai)
                    │                                             │
                    ▼                                             ▼
               [REVISION]                                     [APPROVED]
                    │                                             │
         (Upload Draft Revisi)                        (Creator Tayang di Medsos)
                    │                                             │
                    ▼                                             ▼
               [SUBMITTED]                                   [PUBLISHED]
```
