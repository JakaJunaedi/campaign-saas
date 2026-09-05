# jsreport Template & Data Contract Specification — Campaign SaaS

## 1. Document Control
- **Title**: Report Templates & PDF Contract Specification
- **Purpose**: Mendefinisikan skema JSON payload dan template Handlebars jsreport untuk menghasilkan PDF laporan performa kampanye.
- **Status**: ACCEPTED
- **Scope**: Reporting Module jsreport Worker

---

## 2. JSON Data Payload Contract (Sent to jsreport)

```json
{
  "organization": {
    "name": "Bintang Agency Jakarta",
    "logoUrl": "https://storage.campaignsaas.com/logos/org-01.png"
  },
  "campaign": {
    "title": "Summer Glowing Skincare Launch 2026",
    "clientName": "GlowBeauty Indonesia",
    "startDate": "2026-08-01",
    "endDate": "2026-08-31",
    "totalBudget": 150000000,
    "currency": "IDR"
  },
  "summaryMetrics": {
    "totalCreators": 12,
    "totalDeliverables": 24,
    "totalReach": 1450000,
    "totalImpressions": 2100000,
    "totalViews": 1850000,
    "totalEngagement": 195000,
    "averageEngagementRate": 9.28,
    "costPerEngagement": 769.23,
    "costPerView": 81.08
  },
  "deliverables": [
    {
      "creatorName": "Amanda Putri",
      "platform": "Instagram",
      "contentType": "Reel",
      "liveUrl": "https://instagram.com/reel/xyz123",
      "reach": 250000,
      "views": 320000,
      "likes": 28000,
      "comments": 1400,
      "shares": 3200,
      "totalEngagement": 32600,
      "engagementRate": 10.18
    }
  ]
}
```

---

## 3. Template Engine & Styling Standards
- **Engine**: `handlebars` + `chrome-pdf` recipe di jsreport.
- **Page Format**: A4 Landscape / Portrait dengan header & footer otomatis (Page Numbering & Disclaimer).
- **CSS Framework**: Embedded Tailwind CSS / Custom Inline Print Stylesheet untuk memastikan rendering presisi tinggi.
