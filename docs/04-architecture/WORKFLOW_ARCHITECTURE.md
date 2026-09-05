# Workflow Engine Architecture (Camunda BPMN) — Campaign SaaS

## 1. Document Control
- **Title**: Workflow Architecture & Orchestration Specification
- **Purpose**: Mendefinisikan integrasi Camunda BPMN untuk mengorkestrasi state machine Campaign dan Deliverable Approval.
- **Status**: ACCEPTED
- **Scope**: Workflow Engine Architecture

---

## 2. Prinsip Pemisahan Tanggung Jawab
```text
┌──────────────────────────────────────┐      ┌──────────────────────────────────────┐
│             Domain Layer             │      │          Camunda BPMN Engine         │
│  - Business Rules & Validation       │ ───> │  - Workflow State Orchestration      │
│  - Aggregate Invariants              │      │  - Task Assignment & Due-Date Timers │
│  - State Persistence in PostgreSQL   │      │  - BPMN Visual Execution History     │
└──────────────────────────────────────┘      └──────────────────────────────────────┘
```
- **Aturan Bisnis** tetap berada di Domain Model C# (.NET 10).
- **Camunda** bertanggung jawab mengorkestrasi urutan langkah (*sequencing*), event gateway, dan timer batas waktu.

---

## 3. Deliverable Review & Approval BPMN Process

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

## 4. Integrasi External Worker (.NET 10)
- Aplikasi backend .NET bertindak sebagai **External Task Worker** yang mengeksekusi topik Camunda (contoh: `notify-revision-task`, `generate-report-task`).
- Komunikasi menggunakan REST API Camunda 7 / Zeebe gRPC client.
