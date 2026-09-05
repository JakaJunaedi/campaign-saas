# Microservice Extraction Strategy (Future) — Campaign SaaS

## 1. Document Control
- **Title**: Microservice Evolution & Extraction Plan
- **Purpose**: Panduan dekomposisi Modular Monolith menjadi Microservices di masa depan menggunakan Strangler Pattern.
- **Status**: PROPOSED (Future Architectural Roadmap)
- **Scope**: Post-MVP Evolution

---

## 2. Prinsip Ekstraksi: Evidence-Based Decomposition
Monolith **TIDAK AKAN** dipecah menjadi microservices kecuali memenuhi kriteria terukur:
1. **Workload / Scale Mismatch**: Beban komputasi salah satu modul jauh lebih tinggi (misal: Rendering PDF jsreport atau Video Transcoding).
2. **Team Autonomy**: Tim pengembang terbagi menjadi beberapa skuad mandiri dengan siklus rilis berbeda.
3. **Failure Blast Radius**: Modul membutuhkan isolasi kegagalan agar tidak menjatuhkan modul inti.

---

## 3. Strangler Fig Extraction Pattern

```text
[Existing Monolith Module] ──> [Define Clear Contract / Event] ──> [Deploy Microservice] ──> [Redirect Traffic via Gateway] ──> [Decommission Monolith Code]
```

### Kandidat Ekstraksi Pertama:
1. **`Reporting & Document Service`**: Beban CPU intensif untuk kompilasi data dan render PDF.
2. **`Notification Service`**: Beban I/O tinggi untuk broadcast pesan multi-channel (WhatsApp, Email, Push).
3. **`Creator Portal & Content Intake Service`**: Lalu lintas upload file besar dari ribuan creator independen.
