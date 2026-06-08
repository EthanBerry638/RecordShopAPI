# Record Shop Inventory API

RESTful API for managing a record shop inventory.

---

## Features 🎶

- **Error Handling:** Global exception handling middleware
- **Logging:** Logging middleware that writes request information (HTTP verb and URL path) and response information (status code and elapsed ms)
- **Testing:** Unit and Integration tests for existing endpoints
- **Health Checks:** Endpoint to monitor database health

## Tech Stack 🛠️

- **Language:** C# 11 / .NET 8
- **Database:** EF Core, SQL Server (Production) and SQLite (Development)
- **Testing:** NUnit, Fluent Assertions and Moq

---

## Getting Started 📖

1. Clone the repository.
2. Navigate to the project directory and run the application:
   ```bash
   dotnet run --project RecordShop.Api
   ```
3. Open `https://localhost:7091/swagger` in your browser to view and test the endpoints via Swagger.

**Note:** By default, the API runs in a `Development` environment using an in-memory SQLite database, which is automatically seeded.

---

### Using a Persistent SQL Server Database (Production)

1. Initialize user secrets in the `RecordShop.Api` directory:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=RecordShop;User Id=YOUR_USER;Password=YOUR_PASSWORD;Trust Server Certificate=True;"
   ```
2. Run the application in the Production environment:
   ```bash
   dotnet run --project RecordShop.Api --environment Production
   ```

**Note:** Both the in-memory SQLite database and the persistent SQL Server database will be seeded by default. If you wish to change this, find `database.SeedData()` inside `Program.cs` and either move it to the `else` block or comment/delete it.

---

## How to Run the Tests 🧪

To execute the unit and integration test suites, run the following command from the root directory:
```bash
dotnet test
```

---

## API Endpoints 🔚

### Artists
- **GET** `/api/artists` — Retrieve all artists
- **GET** `/api/artists/{id}` — Retrieve a specific artist by ID
- **POST** `/api/artists` — Add a new artist to the inventory
- **PUT** `/api/artists/{id}` — Update an existing artist's details
- **DELETE** `/api/artists/{id}` — Remove an artist from the inventory

### Albums
- **GET** `/api/albums` — Retrieve all albums
- **GET** `/api/albums/{id}` — Retrieve a specific album by ID
- **POST** `/api/albums` — Add a new album to the inventory
- **PUT** `/api/albums/{id}` — Update an existing album's details
- **DELETE** `/api/albums/{id}` — Remove an album from the inventory

### Health
- **GET** `/health` — Monitor database health

---

## Roadmap 🛣️

- Relational endpoints (e.g., Get Albums by Artist)
- JWT Authentication
- Caching
- Rate limiting