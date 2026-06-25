# FinTech API — Sistema de Gestión de Préstamos

API REST para simulación, solicitud y gestión de préstamos con soporte de transacciones idempotentes, desarrollada en **.NET 10** con **PostgreSQL**.

- [Link de swagger](http://fintech-dev-alb-441114658.us-east-1.elb.amazonaws.com/swagger/index.html)

## 📋 Requisitos

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)

---

## 🚀 Inicio Rápido

### 1. Clonar y restaurar

```bash
git clone <repo-url>
cd prueba-net
dotnet restore
```

### 2. Configurar base de datos

Edita `FinTech.API/appsettings.json` con tu conexión PostgreSQL:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=fintech_db;Username=postgres;Password=postgres"
}
```

### 3. Crear la base de datos

```bash
# Opción A: Directo con psql
psql -U postgres -c "CREATE DATABASE fintech_db;"

# Opción B: Con Docker
docker run -d --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16
```

### 4. Aplicar migraciones

```bash
cd FinTech.API
dotnet ef database update
```

### 5. Ejecutar la API

```bash
dotnet run
```

La API estará disponible en: **http://localhost:5172**

### 6. Acceder a Swagger

Abrir en el navegador: **http://localhost:5172/swagger**

---

## 📚 Documentación de la API

### Autenticación

Todos los endpoints (excepto `/simulate` y `/auth/login`) requieren un token JWT.

**Login:**

```
POST /api/auth/login
Content-Type: application/json

{
  "email": "juan@email.com",
  "password": "11223344"
}
```

**Respuesta:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": "a1b2c3d4-...",
  "name": "Juan Pérez",
  "email": "juan@email.com"
}
```

Para usar en Swagger, haz clic en **Authorize** e ingresa: `Bearer {token}`

### Endpoints de Préstamos

| Método  | Endpoint                   | Auth | Descripción                                       |
| ------- | -------------------------- | ---- | ------------------------------------------------- |
| `POST`  | `/api/loans/simulate`      | ❌   | Simular préstamo (retorna cronograma sin guardar) |
| `POST`  | `/api/loans`               | ✅   | Crear solicitud de préstamo                       |
| `GET`   | `/api/loans?userId=`       | ✅   | Listar préstamos                                  |
| `GET`   | `/api/loans/{id}`          | ✅   | Obtener préstamo por ID                           |
| `GET`   | `/api/loans/{id}/schedule` | ✅   | Obtener cronograma de pagos                       |
| `PATCH` | `/api/loans/{id}/approve`  | ✅   | Aprobar préstamo                                  |
| `PATCH` | `/api/loans/{id}/reject`   | ✅   | Rechazar préstamo                                 |

### Endpoints de Transacciones

| Método | Endpoint                          | Auth | Descripción                             |
| ------ | --------------------------------- | ---- | --------------------------------------- |
| `POST` | `/api/transactions`               | ✅   | Crear transacción (con idempotency_key) |
| `GET`  | `/api/transactions?type=&status=` | ✅   | Listar transacciones                    |
| `GET`  | `/api/transactions/{id}`          | ✅   | Obtener transacción por ID              |

---

## 🧪 Tests

```bash
dotnet test
```

### Tests incluidos (18 tests)

**Unitarios:**

- Cálculo de cuota fija (sistema francés)
- Cálculo de cuota decreciente (sistema alemán)
- Generación de cronograma de pagos
- Validación de monto mínimo/máximo ($500 - $50,000)
- Validación de plazo (6 - 60 meses)
- Cálculo de TEM (Tasa Efectiva Mensual)
- Manejo de fechas fin de mes

**Integración:**

- Deduplicación de transacciones con mismo idempotency_key
- Creación de transacciones con diferentes keys

---

## 🏗️ Arquitectura

```
FinTech.API/
├── Controllers/        → LoansController, TransactionsController, AuthController
├── DTOs/               → Requests/ + Responses/
├── Models/             → Loan, Transaction, PaymentSchedule, User
├── Enums/              → LoanStatus, LoanType, TransactionStatus, TransactionType, PaymentScheduleStatus
├── Services/           → LoanService, TransactionService, AuthService + Interfaces
├── Strategies/         → FixedInstallmentStrategy, DecreasingInstallmentStrategy + Factory
├── Repositories/       → Interfaces/ + Implementations/
├── Data/               → AppDbContext + Configurations EF
├── Middleware/          → ExceptionMiddleware
├── Validators/          → FluentValidation
├── Migrations/          → Migraciones EF Core + Seed Data
└── Utils/               → FinancialCalculator
```

### Patrones de Diseño

| Patrón         | Implementación                                                              |
| -------------- | --------------------------------------------------------------------------- |
| **Repository** | `Repositories/Interfaces/` → `Repositories/Implementations/`                |
| **Strategy**   | `Strategies/` → `FixedInstallmentStrategy`, `DecreasingInstallmentStrategy` |
| **Factory**    | `LoanCalculationFactory` para crear la estrategia según el tipo de préstamo |
| **Middleware** | `ExceptionMiddleware` para manejo global de errores                         |

---

## 🗄️ Modelo de Datos

### Users

| Campo         | Tipo          | Descripción        |
| ------------- | ------------- | ------------------ |
| Id            | UUID          | PK                 |
| Name          | VARCHAR(100)  | Nombre completo    |
| Email         | VARCHAR(150)  | Email (único)      |
| PasswordHash  | VARCHAR(200)  | Hash de contraseña |
| MonthlyIncome | DECIMAL(12,2) | Ingreso mensual    |

### Loans Prestamos

| Campo        | Tipo          | Descripción                            |
| ------------ | ------------- | -------------------------------------- |
| Id           | UUID          | PK                                     |
| UserId       | UUID          | FK → Users                             |
| Amount       | DECIMAL(12,2) | Monto ($500 - $50,000)                 |
| Term         | INT           | Plazo en meses (6 - 60)                |
| InterestRate | DECIMAL(5,4)  | TEA (18% - 35%)                        |
| LoanType     | VARCHAR(20)   | Fixed / Decreasing                     |
| Status       | VARCHAR(20)   | Pending / Approved / Rejected / Active |

### PaymentSchedules

| Campo            | Tipo          | Descripción          |
| ---------------- | ------------- | -------------------- |
| Id               | UUID          | PK                   |
| LoanId           | UUID          | FK → Loans (CASCADE) |
| PaymentNumber    | INT           | Número de cuota      |
| DueDate          | DATE          | Fecha de vencimiento |
| TotalPayment     | DECIMAL(12,2) | Cuota total          |
| Principal        | DECIMAL(12,2) | Amortización capital |
| Interest         | DECIMAL(12,2) | Interés              |
| RemainingBalance | DECIMAL(12,2) | Saldo pendiente      |
| Status           | VARCHAR(20)   | Pending / Paid       |

### Transactions

| Campo          | Tipo          | Descripción                               |
| -------------- | ------------- | ----------------------------------------- |
| Id             | UUID          | PK                                        |
| IdempotencyKey | VARCHAR(100)  | **Unique Index** — garantiza idempotencia |
| Type           | VARCHAR(20)   | Disbursement / Payment / Transfer         |
| Amount         | DECIMAL(12,2) | Monto                                     |
| Status         | VARCHAR(20)   | Pending / Completed / Failed              |
| LoanId         | UUID          | FK → Loans (nullable)                     |
| UserId         | UUID          | FK → Users                                |

---

## 📐 Reglas de Negocio

### Cálculos Financieros

- **TEM** = (1 + TEA)^(1/12) - 1
- **Cuota Fija** (Francés): `Cuota = Monto × [TEM × (1+TEM)^n] / [(1+TEM)^n - 1]`
- **Cuota Decreciente** (Alemán): Amortización constante = Monto / n

### Validaciones

- Monto: $500 - $50,000
- Plazo: 6 - 60 meses
- TEA: 18% - 35%
- Máximo 3 préstamos activos por cliente
- Suma de cuotas ≤ 40% de ingresos mensuales

### Flujo de Aprobación

```
PENDING → [APPROVED | REJECTED] → ACTIVE
```

**Auto-aprobación:** Monto < $10,000 y menos de 2 préstamos activos → automático.
**Desembolso:** Al aprobar se crea automáticamente una transacción de tipo `Disbursement`.

### Idempotencia

- Cada transacción tiene un `IdempotencyKey` único
- Si llega una segunda request con la misma key, se retorna la transacción original sin duplicar

## 🔧 Tecnologías

| Tecnología            | Versión |
| --------------------- | ------- |
| .NET                  | 10.0    |
| ASP.NET Core          | 10.0    |
| Entity Framework Core | 10.0    |
| PostgreSQL (Npgsql)   | 10.0    |
| Swashbuckle (Swagger) | 10.2    |
| FluentValidation      | 11.11   |
| JWT Bearer            | 10.0    |
| xUnit                 | (tests) |
| Moq                   | (tests) |
| FluentAssertions      | (tests) |

---

## 📁 Estructura del Proyecto

```
prueba-net/
├── FinTech.slnx
├── FinTech.API/              → Proyecto Web API
│   ├── Controllers/
│   ├── DTOs/
│   ├── Models/
│   ├── Enums/
│   ├── Services/
│   ├── Strategies/
│   ├── Repositories/
│   ├── Data/
│   ├── Middleware/
│   ├── Validators/
│   ├── Migrations/
│   ├── Utils/
│   ├── Program.cs
│   └── appsettings.json
├── FinTech.Tests/            → Proyecto de Tests
│   ├── UnitTests/
│   └── IntegrationTests/
└── README.md
```
