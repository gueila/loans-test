# FinTech API — Sistema de Gestión de Préstamos

API REST para simulación, solicitud y gestión de préstamos con soporte de transacciones idempotentes, desarrollada en **.NET 10** con **PostgreSQL**.

---

## 📋 Requisitos

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- Opcional: [Docker](https://www.docker.com/) (para PostgreSQL)

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

| Método | Endpoint | Auth | Descripción |
|--------|----------|------|-------------|
| `POST` | `/api/loans/simulate` | ❌ | Simular préstamo (retorna cronograma sin guardar) |
| `POST` | `/api/loans` | ✅ | Crear solicitud de préstamo |
| `GET` | `/api/loans?userId=` | ✅ | Listar préstamos |
| `GET` | `/api/loans/{id}` | ✅ | Obtener préstamo por ID |
| `GET` | `/api/loans/{id}/schedule` | ✅ | Obtener cronograma de pagos |
| `PATCH` | `/api/loans/{id}/approve` | ✅ | Aprobar préstamo |
| `PATCH` | `/api/loans/{id}/reject` | ✅ | Rechazar préstamo |

### Endpoints de Transacciones

| Método | Endpoint | Auth | Descripción |
|--------|----------|------|-------------|
| `POST` | `/api/transactions` | ✅ | Crear transacción (con idempotency_key) |
| `GET` | `/api/transactions?type=&status=` | ✅ | Listar transacciones |
| `GET` | `/api/transactions/{id}` | ✅ | Obtener transacción por ID |

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

| Patrón | Implementación |
|--------|---------------|
| **Repository** | `Repositories/Interfaces/` → `Repositories/Implementations/` |
| **Strategy** | `Strategies/` → `FixedInstallmentStrategy`, `DecreasingInstallmentStrategy` |
| **Factory** | `LoanCalculationFactory` para crear la estrategia según el tipo de préstamo |
| **Middleware** | `ExceptionMiddleware` para manejo global de errores |

---

## 🗄️ Modelo de Datos

### Users
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(100) | Nombre completo |
| Email | VARCHAR(150) | Email (único) |
| PasswordHash | VARCHAR(200) | Hash de contraseña |
| MonthlyIncome | DECIMAL(12,2) | Ingreso mensual |

### Loans Prestamos
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → Users |
| Amount | DECIMAL(12,2) | Monto ($500 - $50,000) |
| Term | INT | Plazo en meses (6 - 60) |
| InterestRate | DECIMAL(5,4) | TEA (18% - 35%) |
| LoanType | VARCHAR(20) | Fixed / Decreasing |
| Status | VARCHAR(20) | Pending / Approved / Rejected / Active |

### PaymentSchedules
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | UUID | PK |
| LoanId | UUID | FK → Loans (CASCADE) |
| PaymentNumber | INT | Número de cuota |
| DueDate | DATE | Fecha de vencimiento |
| TotalPayment | DECIMAL(12,2) | Cuota total |
| Principal | DECIMAL(12,2) | Amortización capital |
| Interest | DECIMAL(12,2) | Interés |
| RemainingBalance | DECIMAL(12,2) | Saldo pendiente |
| Status | VARCHAR(20) | Pending / Paid |

### Transactions
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | UUID | PK |
| IdempotencyKey | VARCHAR(100) | **Unique Index** — garantiza idempotencia |
| Type | VARCHAR(20) | Disbursement / Payment / Transfer |
| Amount | DECIMAL(12,2) | Monto |
| Status | VARCHAR(20) | Pending / Completed / Failed |
| LoanId | UUID | FK → Loans (nullable) |
| UserId | UUID | FK → Users |

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

---

## 🐳 Docker

### Dockerfile

Multi-stage build para mantener la imagen final pequeña (~200MB).

```dockerfile
# FASE 1: Build — compila la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY FinTech.API/FinTech.API.csproj .
RUN dotnet restore                              # Restaura dependencias (cacheable)
COPY . .
RUN dotnet publish FinTech.API/FinTech.API.csproj -c Release -o /app

# FASE 2: Runtime — solo lo necesario para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .                        # Copia solo el binario compilado

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FinTech.API.dll"]
```

**Build local:**
```bash
docker build -t fintech-api .
docker run -p 8080:8080 fintech-api
# → http://localhost:8080/swagger
```

---

## ☁️ Deploy AWS (ECS Fargate + RDS)

### Infraestructura como Código (Terraform)

```
terraform/
├── main.tf              → Provider AWS
├── variables.tf         → 18 variables configurables
├── outputs.tf           → ALB DNS, ECR URL, RDS endpoint
├── networking.tf        → VPC, subnets (2 públicas + 2 privadas), NAT, security groups
├── secrets.tf           → AWS Secrets Manager (db-connection + jwt)
├── iam.tf               → Roles ECS execution + task
├── rds.tf               → RDS PostgreSQL 16, t3.micro, 20GB gp3
├── ecs.tf               → ECR, ECS Fargate 0.25vCPU/512MB, ALB, auto-scaling
└── terraform.tfvars     → Variables por defecto
```

### Arquitectura AWS

```
Internet
   │
   ▼
  ALB (Application Load Balancer) — puertos 80/443
   │
   ▼
  ECS Fargate (contenedor .NET)
   │                        │
   ▼                        ▼
  RDS PostgreSQL         Secrets Manager
  (privada, sin         (db-connection, jwt)
   acceso internet)
```

### Recursos creados

| Recurso | Descripción |
|---------|-------------|
| **VPC** | 2 subnets públicas (ALB), 2 privadas (ECS + RDS) |
| **Security Groups** | ALB (80/443 desde internet), ECS (solo desde ALB), RDS (5432 solo desde ECS) |
| **RDS PostgreSQL 16** | t3.micro, 20GB gp3, backups 7 días, cifrado |
| **Secrets Manager** | `fintech/{env}/db-connection` (connection string), `fintech/{env}/jwt` (key, issuer, audience) |
| **ECR** | Repositorio privado de imágenes Docker |
| **ECS Fargate** | 0.25 vCPU, 512MB RAM, auto-escalado (CPU > 70%, min 1 — max 3) |
| **ALB** | Balanceador HTTP, health check en `/swagger/index.html` |
| **CloudWatch Logs** | Logs del contenedor con retención de 14 días |

### Secrets Management

Los secrets se almacenan en **AWS Secrets Manager** y ECS los inyecta como variables de entorno automáticamente:

```json
// Secret: fintech/dev/db-connection
"Host=fintech-db.xxx.us-east-1.rds.amazonaws.com;Port=5432;Database=fintech_db;Username=postgres;Password=xxx"

// Secret: fintech/dev/jwt
{
  "key": "clave-segura-32-caracteres-minimo!",
  "issuer": "FinTech.API",
  "audience": "FinTech.App"
}
```

### Deploy paso a paso

```bash
# 1️⃣  Variables sensibles (nunca en git)
export TF_VAR_db_password="supersecreta"
export TF_VAR_jwt_key="clave-jwt-de-32-caracteres-minimo"

# 2️⃣  Crear infraestructura
cd terraform
terraform init
terraform plan          # Revisar qué va a crear
terraform apply         # Aprobar con "yes"

# 3️⃣  Buildear imagen Docker y pushear a ECR
aws ecr get-login-password --region us-east-1 | docker login \
  --username AWS --password-stdin $(terraform output -raw ecr_repository_url)

docker build -t fintech-api ../
docker tag fintech-api:latest $(terraform output -raw ecr_repository_url):latest
docker push $(terraform output -raw ecr_repository_url):latest

# 4️⃣  Forzar nuevo deploy en ECS
aws ecs update-service \
  --cluster fintech-dev-cluster \
  --service fintech-dev-service \
  --force-new-deployment

# 5️⃣  Verificar
echo "API disponible en: http://$(terraform output -raw alb_dns_name)/swagger"
```

### CI/CD (GitHub Actions — opcional)

```yaml
name: Deploy
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0'

      - name: Run tests
        run: dotnet test

      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v4
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: us-east-1

      - name: Login to ECR
        run: aws ecr get-login-password | docker login --username AWS --password-stdin ${{ secrets.ECR_REPOSITORY }}

      - name: Build and push
        run: |
          docker build -t fintech-api .
          docker tag fintech-api:latest ${{ secrets.ECR_REPOSITORY }}:latest
          docker push ${{ secrets.ECR_REPOSITORY }}:latest

      - name: Deploy to ECS
        run: |
          aws ecs update-service --cluster fintech-dev-cluster \
            --service fintech-dev-service --force-new-deployment
```

### Costos estimados (dev)

| Servicio | Costo mensual |
|----------|--------------|
| RDS db.t3.micro | ~$15 |
| ECS Fargate 0.25vCPU + 0.5GB | ~$10 |
| ALB | ~$16 |
| Secrets Manager (2 secrets) | ~$1 |
| **Total** | **~$42/mes** |

---

## 🔧 Tecnologías

| Tecnología | Versión |
|------------|---------|
| .NET | 10.0 |
| ASP.NET Core | 10.0 |
| Entity Framework Core | 10.0 |
| PostgreSQL (Npgsql) | 10.0 |
| Swashbuckle (Swagger) | 10.2 |
| FluentValidation | 11.11 | 
| JWT Bearer | 10.0 |
| xUnit | (tests) |
| Moq | (tests) |
| FluentAssertions | (tests) |

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
