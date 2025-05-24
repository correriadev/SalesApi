# Sales API

A microservices-based sales application with API Gateway pattern implementation.

## Project Structure

```
SalesApi/
├── src/
│   ├── Gateway/                      # API Gateway using Ocelot
│   └── SalesApi/
│       ├── SalesApi.WebApi/          # Web API Layer
│       ├── SalesApi.Worker/          # Background Worker (subscriber)
│       ├── SalesApi.Domain/          # Domain Layer
│       ├── SalesApi.Application/     # Application Layer (CQRS, MediatR, Handlers)
│       ├── SalesApi.Infrastructure/  # Infrastructure Layer (shared infra)
│       ├── SalesApi.Infrastructure.Bus/      # Message Bus (Rebus, RabbitMQ)
│       ├── SalesApi.Infrastructure.Data.Sql/ # SQL Data Access
│       ├── SalesApi.ViewModel/       # ViewModel Layer (DTOs)
│
├── tests/
│   ├── Application/                 # Unit tests for Application layer (commands/queries)
│   ├── Integration/                  # Integration tests (API, end-to-end)
│   ├── WebApi/                       # Web API specific tests
│   ├── Infrastructure/               # Infrastructure layer tests
│   ├── Common/                       # Shared test utilities
│   ├── Domain/                       # Domain layer tests

│
├── docker-compose.yml                # Docker services configuration
├── docker-compose.override.yml       # Development-specific Docker settings
└── Directory.Packages.props          # Central package management
```

*Note: Some folders (obj, bin, results, Common) are for test support or build artifacts and may not contain source code.*

## Technologies

- .NET 8.0
- Docker & Docker Compose
- Ocelot API Gateway
- PostgreSQL
- Health Checks
- Rebus (RabbitMQ message bus)
- xUnit (testing)

## Prerequisites

- Docker Desktop
- .NET 8.0 SDK
- Git

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/correriadev/SalesApi.git
cd SalesApi
```

2. Build and run the Docker containers:
```bash
docker-compose up --build
```

The services will be available at:
- API Gateway: http://localhost:7777
- Sales API: http://localhost:8090
- PostgreSQL: localhost:5432

## Message Bus (Rebus + RabbitMQ)

- The solution uses [Rebus](https://github.com/rebus-org/Rebus) with RabbitMQ for asynchronous messaging.
- **Publisher/Subscriber separation:**
  - The WebApi is configured as a publisher (sends commands/events).
  - The Worker is configured as a subscriber (handles commands/events).
- **Configuration:**
  - Both publisher and subscriber use RabbitMQ transport, System.Text.Json serialization, console logging, and a simple retry strategy.
  - Only command types (e.g., `CreateProductMessage`, `CreateSaleMessage`) are mapped to the queue.
  - Subscribers use `services.AutoRegisterHandlersFromAssemblyOf<...>()` to wire up all `IHandleMessages<T>` handlers.
  - After building the host, `app.Services.StartMessageBusSubscriptionsAsync()` is called to subscribe to each message type.

## Validation in Command Handlers

- All command handlers (e.g., `CreateProductCommandHandler`, `CreateSaleCommandHandler`) perform input validation and throw `ArgumentException` for invalid input (e.g., empty fields, negative price, empty items).
- This ensures that only valid data is processed and persisted.

## Testing

### Test Structure
- **Unit tests for Application layer:**
  - Located in `tests/Application/`
  - Cover command and query handlers (e.g., `CreateProductCommandHandlerTests`, `GetAllProductsQueryHandlerTests`, `CreateSaleCommandHandlerTests`)
- **Integration tests:**
  - Located in `tests/Integration/`
  - Cover end-to-end scenarios and API endpoints

### Running Tests

To run all tests:
```bash
dotnet test
```

- Test order for integration tests is controlled by a custom orderer and `[TestPriority]` attribute.
- Parallelization is disabled for integration tests to ensure order is respected.
- Check the test output for the order of execution.

### Adding New Tests

- To add a new test for a command or query handler:
  1. Create a new test class in the appropriate folder under `tests/Application/`.
  2. Use NSubstitute to mock repositories and dependencies.
  3. Use xUnit `[Fact]` or `[Theory]` for your test methods.
  4. Assert on the result and expected exceptions/validation.

## Available Endpoints

### Health Check
- Gateway: `GET http://localhost:7777/health`
- Sales API: `GET http://localhost:8090/health`

## Development

### Project Structure
- **Gateway**: Handles routing and API aggregation using Ocelot
- **SalesApi.WebApi**: Main API implementation
- **SalesApi.Domain**: Business logic and domain models
- **SalesApi.Infrastructure**: Data access and external services
- **SalesApi.Application**: CQRS, MediatR, and business logic handlers

### Docker Commands

- Start all services:
```bash
docker-compose up
```

- Rebuild and start services:
```bash
docker-compose up --build
```

- Stop all services:
```bash
docker-compose down
```

- View logs:
```bash
docker-compose logs -f
```

## Environment Variables

### Sales API
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: http://+:8090
- `ConnectionStrings__SalesApiDb`: Database connection string

### Gateway
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: http://+:7777

### Database
- `POSTGRES_DB`: SalesApiDb
- `POSTGRES_USER`: postgres
- `POSTGRES_PASSWORD`: admin

## Contributing

1. Create a new branch for your feature
2. Make your changes
3. Submit a pull request

## License

MIT License

Copyright (c) [year] [fullname]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
