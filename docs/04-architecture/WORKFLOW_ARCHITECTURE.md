# Workflow Engine Architecture (Camunda BPMN) — Campaign SaaS

## 1. Document Control
- **Title**: Workflow Architecture & Orchestration Specification
- **Purpose**: Mendefinisikan arsitektur workflow state machine untuk siklus campaign dan deliverable approval (MVP DDD + Phase 6 BPMN).
- **Status**: ACCEPTED (Phased Execution)
- **Scope**: Workflow Engine Architecture

---

## 2. Dual-Phase Workflow Execution Strategy

Sistem dirancang dengan arsitektur dua fase untuk memisahkan domain logic murni dari external orchestration engine:

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│                       FASE MVP (Phase 1 - Phase 5)                          │
│                                                                             │
│  [Domain Entities] ────(Domain Events)───> [MassTransit Transactional Outbox]│
│  - State Invariants                        - Asynchronous RabbitMQ Publish  │
│  - In-Process State Machine                - Event Choreography & Audit Log │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼ (Post-MVP / Phase 6 Expansion)
┌─────────────────────────────────────────────────────────────────────────────┐
│                    FASE POST-MVP / ENTERPRISE (Phase 6)                     │
│                                                                             │
│  [Camunda 7.20 / Zeebe Engine] <──(External Tasks)──> [.NET Worker Service]│
│  - BPMN Visual Execution Diagram                      - Task Topic Handlers │
│  - Automated SLA Timers & Escalations                 - Domain Integration  │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Deliverable Review & Approval BPMN Process Flow

```text
[Start: Deliverable Assigned]
       ↓
(Wait Task: Creator Submit Content)
       ↓
[Event: ContentSubmitted]
       ↓
(User Task: Internal Reviewer Review)
       ├── [Decision: Revision Requested] ──> (Timer / Reminder) ──> (Wait Task: Creator Resubmit) ──┐
       │                                                                                             │
       ├── [Decision: Approved] ──> (Wait Task: Creator Publish & Submit Live URL)                   │
       │                                         ↓                                                   │
       │                              [Event: ContentPublished]                                     │
       │                                         ↓                                                   │
       │                              [End: Deliverable Completed]                                   │
       │                                                                                             │
       └── [Decision: Rejected] ──> [End: Deliverable Cancelled]                                     │
                                         ▲                                                           │
                                         └───────────────────────────────────────────────────────────┘
```

---

## 4. Implementasi Saat Ini vs Phase 6
- **MVP (Active)**: Menggunakan C# Domain Aggregates (`Deliverable`, `ContentSubmission`, `ApprovalReview`), CQRS Handlers, dan MassTransit Transactional Outbox.
- **Phase 6 Target**: `CampaignSaaS.Worker` mengeksekusi topik Camunda External Task (`content-review-task`, `reminder-due-task`, `pdf-report-task`).

