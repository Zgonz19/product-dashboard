# Product Management App — .NET API + Angular


## 1. Quick Start

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- Angular CLI (`npm install -g @angular/cli`)
- Visual Studio or VS Code

### Run Instructions

#### Backend (.NET API)

```bash
cd ProductAPI
dotnet run --project ProductAPI
```

Visit `https://localhost:7270/swagger/index.html` to view swagger page

#### Frontend (Angular)

```bash
cd ProductApp
npm install
ng serve
```

Visit `http://localhost:4200` to view the product list.


## 2. Architecture

### Overall Architecture Approach

- Backend: Clean separation of concerns with Controllers → Services → EF Core DbContext
- Frontend: Angular 17+ standalone components and functional routing
- Integration: RESTful API with `x-api-key` header authentication

### Database Schema

- Products: Id (PK), CategoryId (FK), Name, Description, Price, StockQuantity, CreatedDate, IsActive
	Indexes: CategoryId, Name, Price, StockQuantity, IsActive
	
- Categories: Id (PK), Name, Description, IsActive
	Indexes: Name, IsActive
	
One-to-many (Category → Products)

### Technology Choices

| Layer       | Tech Stack                     |
|-------------|--------------------------------|
| Backend     | .NET 8 Web API, EF Core        |
| Database    | SQLite                         |
| Auth        | API Key via `x-api-key` header |
| Frontend    | Angular 17+, TypeScript        |
| Tooling     | Angular CLI, EF CLI, VS Code   |


## 3. Design Decisions

### Single Responsibility & Dependency Inversion

- Controllers depend on abstractions (`ICategoryService`, `IProductService`)
- Angular services injected via DI for clean separation

### EF Core Approach & Query Optimization

- Explicit model configuration with Fluent API
- Async queries with `AsNoTracking()` where applicable
- Controlled eager loading via `.Include()` only when needed
- Using code first database migrations

### Complex Endpoint Choice

- Option A - Product Search
- Supports real-world search behavior on multiple fields and keywords with pagination

### DbContext via Service layers instead of Repository Pattern

- Avoiding unnecessary abstraction
- Consider Repository Pattern when:
	Duplication of the same EF Core queries throughout the Service layer
	Business Logic should be decoupled from data access layer
	Multiple data access layers or plans to migrate database from one service to another (eg., MySql -> PostgreSQL)

### Index Strategy

- Indexing columns used by Product Search endpoint
- Indexed `CategoryId` in `Products` for join performance
- Prioritises read over write performance
- Usage insights would give context for better index optimization


## 4. What I Would Do With More Time

### Unimplemented Features

- Product form
- Details view
- Search and filtering

### Refactoring Priorities

- Consider refactoring to Repository Pattern as Business Logic continues to grow
- Consider using an Angular component library 
- Handle ArgumentException in Exception Handling middleware
- On Update Product endpoint, update only specified fields instead of passing the whole Dto.

### Production Considerations

- x-api-key not secure with Angular SPA. Use secure login flow (e.g., JWT, OAuth) to access private endpoints
- Use Key Vault for secrets
- Add Logging and Monitoring via Azure Application Insights


## 5. Assumptions & Trade-offs

### Key Assumptions

- The API and frontend are deployed separately and communicate over HTTP
- The application does not require multi-tenancy, user authentication, or role-based access control

### Trade-offs

- Chose SQLite over SQL Server to simplify local development, portability, and setup.
- Accepted SQLite’s limitations in handling concurrent writes and high-throughput scenarios.
- For production scalability, a full-featured database like SQL Server or PostgreSQL would be required.
- Skipped the Repository Pattern to avoid unnecessary abstraction and boilerplate.