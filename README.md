# Games Service API

A RESTful API service for managing video game information, built with ASP.NET Core and following clean architecture principles.

## Tech Stack

### Framework & Runtime
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - For building HTTP APIs

### Database
- **SQLite** - Lightweight relational database
- **Entity Framework Core 9.0.4** - ORM for database operations

### Architecture & Patterns
- **Repository Pattern** - Generic repository for data access abstraction
- **Service Layer** - Business logic separation
- **DTOs (Data Transfer Objects)** - For request/response models
- **Dependency Injection** - Built-in ASP.NET Core DI container
- **Exception Middleware** - Centralized error handling

### Testing
- **xUnit** - Testing framework
- **Moq** - Mocking library for unit tests


## Project Structure

```
GamesService/
├── Controllers/        # API endpoints
├── Services/           # Business logic
├── Repositories/       # Data access layer
├── Models/             # Domain entities
├── DTOs/               # Data transfer objects
├── Mappers/            # Entity-DTO mapping
├── Middleware/         # Custom middleware (exception handling)
├── Data/               # DbContext and database configuration
└── Exceptions/         # Custom exception classes

GamesService.Tests/
├── Controllers/        # Controller unit tests
├── Services/           # Service unit tests
├── Repositories/       # Repository unit tests
└── Mappers/            # Mapper unit tests
```

## Features

- **CRUD Operations** for video games
- **Data Validation** with data annotations
- **Global Exception Handling** with custom middleware
- **Generic Repository Pattern** for reusable data access
- **Comprehensive Unit Tests** with high code coverage

## Game Model

Each game includes:
- Name
- Genre
- Age Rating
- Price
- Description
- Author/Publisher

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/games` | Get all games |
| GET | `/api/games/{id}` | Get game by ID |
| POST | `/api/games` | Create a new game |
| PUT | `/api/games/{id}` | Update an existing game |
| DELETE | `/api/games/{id}` | Delete a game |

## Getting Started

### Prerequisites
- .NET 9.0 SDK

### Running the Application

1. Clone the repository
2. Navigate to the solution directory:
   ```bash
   cd newton-games-service
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update --project GamesService
   ```

4. Run the application:
   ```bash
   dotnet run --project GamesService
   ```

5. The API will be available at `http://localhost:5265`

### Running Tests

```bash
dotnet test
```

## Development

The application uses SQLite as its database (`games.db`), which is created automatically when you run migrations. Connection string is configured in `appsettings.json`.
