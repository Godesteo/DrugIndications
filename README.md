# Drug Indications Microservice

This project implements a .NET-based microservice to extract drug indications from DailyMed, map them to ICD-10 codes using AI, and manage structured copay program data via a RESTful API.

---

## 📦 Technologies

- **.NET 8**, **C# 12**
- **Clean Architecture**
- **Docker & Docker Compose**
- **JWT Authentication with Role-based Access**
- **Generative AI (OpenAI)**
- **Swagger for API Documentation**

---
## ✅ Features Implemented

### ✔️ Drug Copay Program API
- CRUD endpoints for `DrugProgram`
- Automatic parsing of free-text eligibility into structured rules using OpenAI
- Swagger documentation

### ✔️ Indication Extraction
- AI-powered extraction of indications from drug label text
- ICD-10 code mapping

### ✔️ Authentication & Authorization
- JWT authentication
- Role-based access (Admin / User)

### ✔️ AI Fallback
- Rule-based fallback if OpenAI quota is exceeded

---

## 🚀 Setup Instructions

### 1. Clone & Run
```bash
git clone https://github.com/your-username/drug-indications.git
cd drug-indications
docker-compose up --build
```

> The API will be available at `http://localhost:5000/swagger`

### 2. Authentication
To log in and receive a token:

```json
POST /auth/login
{
  "username": "admin",
  "password": "admin123"
}
```

Use the JWT token returned to authorize requests in Swagger (🔒 icon).

---

## 🧪 Sample Endpoints

### 🔹 POST /auth/login
Returns JWT token.

### 🔹 GET /programs/{id}
Fetch structured drug program data by ID.

### 🔹 POST /programs
Create a new copay program from structured JSON.
> If eligibility requirements are not included, they are parsed using AI.

⚠️ **Note:** The `/programs` endpoint expects structured data only. It does not support importing the original `dupixent.json` file directly. For that, use:

### 🔹 POST /programs/import
Loads the original `dupixent.json` structure, parses it, and stores it in the database.

```bash
POST /programs/import
```

### 🔹 GET /programs/{id}/parsed-eligibility
Returns AI-parsed eligibility rules for a program.

### 🔹 POST /indications/extract
Extracts medical indications from free-text label and maps them to ICD-10.

---

## 🤖 AI Features
- OpenAI is used to transform unstructured eligibility text into structured JSON rules.
- When OpenAI is unavailable (quota exceeded), a rule-based fallback system activates.

---

## 📂 Project Structure
```
src/
├── DrugIndications.API           → Controllers & Swagger setup
├── DrugIndications.Application  → Services, DTOs, Interfaces
├── DrugIndications.Domain       → Entities & Interfaces
├── DrugIndications.Infrastructure → Repositories & SQL access
```

---

## 🧪 Tests
Test-driven development is applied across the business logic and AI extraction layers.
(You can run tests with `dotnet test` if implemented.)
