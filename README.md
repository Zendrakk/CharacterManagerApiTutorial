# CharacterManagerApiTutorial

A backend tutorial project for managing characters in a fantasy-themed game using **ASP.NET Core 9** and **Entity Framework Core**. This project demonstrates best practices for building modern APIs, including clean architecture, validation logic, and efficient data access patterns.

It supports secure user registration, login, token refresh, and full CRUD operations for character-related data such as races, classes, factions, and realms.

Since this project was built for educational purposes, it includes **extensive inline comments** throughout the code to explain key concepts, logic, and design decisions.

## 🚀 Features

- User registration & JWT login
- Access + Refresh token generation
- Rate-limited refresh endpoint
- Character CRUD operations
- Metadata for races, classes, factions, realms
- Clean service architecture
- Unit tested controllers and services
- Swagger UI

## 🧰 Tech Stack

- ASP.NET Core 9
- Entity Framework Core
- xUnit
- Swagger / OpenAPI
- SQL Server
- JWT Authentication

## ✅ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- Visual Studio or VS Code

## 🧱 Project Structure

```
CharacterManagerApiTutorial/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── CharacterController.cs
│   └── LookupDataController.cs
│
├── Models/
│   ├── Character.cs
│   ├── CharacterDisplayDto.cs
│   ├── CharacterDto.cs
│   ├── CharacterMappings.cs
│   ├── ClassType.cs
│   ├── FactionType.cs
│   ├── RaceType.cs
│   ├── Realm.cs
|   ├── Result.cs
|   ├── LookupDataDto.cs
|   └── Auth/
|       ├── LogoutRequest.cs
|       ├── RefreshTokenRequestDto.cs
|       ├── TokenResponseDto.cs
|       ├── User.cs
│       └── UserDto.cs
│
├── Services/
│   ├── AuthService.cs / IAuthService.cs
│   ├── CharacterService.cs / ICharacterService.cs
│   └── LookupDataService.cs / ILookupDataService.cs
│
├── Data/
│   └── CharacterManagerDbContext.cs
│
├── Migrations/
│   └── Initial migration files
│
├── Tests/
│   ├── Controllers/
│   │   └── AuthControllerTests.cs
│   └── Services/
│       ├── AuthTests.cs
│       └── CharacterTests.cs
```

## 🔍 API Documentation with Swagger UI

This project uses **Swagger (OpenAPI)** to provide built-in, interactive API documentation.

### ✅ Swagger Features
- Fully interactive documentation for all API endpoints
- Auto-generated OpenAPI spec
- **JWT Bearer token authentication** support
- Runs automatically in **Development** environment
- Accessible from the root of the app (`/`)

---

## 🔐 API Endpoints

All endpoints are prefixed with `/api`.

### Auth

| Method | Endpoint              | Description                |
|--------|-----------------------|----------------------------|
| POST   | `/auth/register`      | Register new users         |
| POST   | `/auth/login`         | Authenticate and get JWT   |
| POST   | `/auth/refresh-token` | Refresh access token       |
| POST   | `/auth/logout`        | Revoke refresh token in DB |

### Characters

| Method | Endpoint           | Description              |
|--------|--------------------|--------------------------|
| GET    | `/character`      | Get all characters       |
| GET    | `/character/{id}` | Get character by Id      |
| POST   | `/character`      | Create new character     |
| PUT    | `/character/{id}` | Update a character       |
| DELETE | `/character/{id}` | Delete a character       |

### LookupData

| Method | Endpoint             | Description                    |
|--------|----------------------|--------------------------------|
| GET    | `/lookupdata`        | Get character metadata         |

## 🚫 Character Restrictions

The API enforces the following rules when creating or updating characters:

## ⚔️ Faction-Race-Class Restrictions

The `CharacterService.cs` enforces rules to maintain fantasy-world consistency between a character’s race, class, and faction.

- **Faction** must be one of the allowed factions.
- Each **faction** has specific allowed **races**.
- Each **race** is restricted to certain **classes**.

These rules are enforced when creating or updating a character. If a character's attributes violate these constraints, the service returns a failure result, preventing invalid characters from being saved.

This helps maintain narrative and gameplay consistency in any game built on top of this API.

### Level Restrictions

- Character **Level** range must be between `1` and `50`. Restriction at model level and service level for demonstration purposes.

### Name Length Restrictions

- Character **Name** length must be between `3` and `15` characters. Restriction at model level and service level for demonstration purposes.

### Username Length Restrictions

- **Username** length must be between `3` and `20` characters. Restriction at model level and service level for demonstration purposes.

## 🛠 How to Run

1. Clone the repository
   ```bash
   git clone https://github.com/Zendrakk/CharacterManagerApiTutorial.git
   cd CharacterManagerApiTutorial
   ```

2. Update `appsettings.json` with your SQL Server connection string (or SQL Express)

3. Run database migrations
   ```bash
   dotnet ef database update --project CharacterManagerApiTutorial
   ```

4. Run the app
   ```bash
   dotnet run --project CharacterManagerApiTutorial --launch-profile https
   ```
   To access the `Swagger UI`, open your browser and navigate to either of the urls displayed in your CMD/Bash:
   ```bash
   info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7234
   info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5089
   info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
   ```

5. Run tests
   ```bash
   dotnet test
   ```

## 🧪 Test Coverage

- Controllers: `AuthControllerTests.cs` (TODO: tests for the other controllers)
- Services:
  - `AuthTests.cs`
  - `CharacterTests.cs`

## 📬 Testing the API with `CharacterManagerApiTutorial.http`

This project includes a [**CharacterManagerApiTutorial.http**](./CharacterManagerApiTutorial.http) file that lets you test the API directly within supported IDEs like **Visual Studio** or **VS Code** (with the [REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)).

### 🛠 How to Use

1. Open `CharacterManagerApiTutorial.http` in your IDE.
2. Update the placeholder:
   - Replace `@jwtToken = your-jwt-token-here` with your real token after login.
3. Click "Send Request" above each request to execute it.

### 📄 Available Requests

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/register` | `POST` | Register a new user |
| `/api/auth/login` | `POST` | Authenticate and receive JWT |
| `/api/character` | `GET` | Retrieve all characters (requires token) |
| `/api/character/{id}` | `GET` | Retrieve character by ID |
| `/api/character` | `POST` | Create a new character |
| `/api/character/{id}` | `PUT` | Update an existing character |
| `/api/character/{id}` | `DELETE` | Delete a character |

All secured endpoints require the `Authorization: Bearer {{jwtToken}}` header.

## 📝 License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.

## 👤 Author

**Cliff Nieman**  
GitHub: [@Zendrakk](https://github.com/Zendrakk) 
