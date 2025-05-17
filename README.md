# Sales API

A microservices-based sales application with API Gateway pattern implementation.

## Project Structure

```
SalesApi/
├── src/
│   ├── Gateway/                 # API Gateway using Ocelot
│   │   ├── Gateway.csproj
│   │   ├── Program.cs
│   │   └── ocelot.json         # Gateway routing configuration
│   │
│   └── SalesApi/               # Main API Project
│       ├── SalesApi.WebApi/    # Web API Layer
│       ├── SalesApi.Domain/    # Domain Layer
│       └── SalesApi.Infrastructure/ # Infrastructure Layer
│
├── docker-compose.yml          # Docker services configuration
├── docker-compose.override.yml # Development-specific Docker settings
└── Directory.Packages.props    # Central package management
```

## Technologies

- .NET 8.0
- Docker & Docker Compose
- Ocelot API Gateway
- PostgreSQL
- Health Checks

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
