# Error Handling & RFC 7807 Standards — Campaign SaaS

## 1. Document Control
- **Title**: Error Handling & ProblemDetails Specification
- **Purpose**: Standarisasi format respons error, status code HTTP, dan exception mapping.
- **Status**: ACCEPTED
- **Scope**: API Error Pipeline

---

## 2. Format Respons Error (RFC 7807 ProblemDetails)
Untuk respons error (4xx dan 5xx), sistem menggunakan format standar RFC 7807:

```json
{
  "type": "https://api.campaignsaas.com/errors/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The payload contains invalid field values.",
  "instance": "/api/v1/campaigns",
  "errors": {
    "Title": ["The Title field is required."],
    "Budget": ["Budget must be greater than zero."]
  },
  "traceId": "00-987654321-abcdef-00"
}
```

---

## 3. Mapping HTTP Status Code & Result Pattern

| Domain / Application Result | HTTP Status | Keterangan |
| :--- | :--- | :--- |
| `Result.Success(data)` | **200 OK / 201 Created** | Operasi sukses |
| `Result.Invalid(errors)` | **400 Bad Request** | Validasi input gagal (FluentValidation) |
| `Result.Unauthorized()` | **401 Unauthorized** | Token JWT hilang, expired, atau tidak valid |
| `Result.Forbidden()` | **403 Forbidden** | User tidak memiliki izin peran atau beda tenant |
| `Result.NotFound()` | **404 Not Found** | Resource entitas tidak ditemukan |
| `Result.Conflict()` | **409 Conflict** | Pelanggaran invariant/keadaan unik (misal: duplikasi email) |
| `Unhandled Exception` | **500 Internal Server Error** | Exception sistem (ditangani oleh Global Middleware) |
