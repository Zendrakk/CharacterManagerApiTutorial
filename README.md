# CharacterManagerApiTutorial

A .NET 9 Web API project for managing characters in a fantasy game. It supports secure user registration, login, token refresh, and management of character data like races, classes, and factions.

## 🚀 Features

- User registration & JWT login
- Access + Refresh token generation
- Rate-limited refresh endpoint
- Character CRUD operations
- Metadata for races, classes, factions, realms
- Clean service architecture
- Unit tested controllers and services

## 🧱 Project Structure

```
CharacterManagerApiTutorial/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── CharacterController.cs
│   └── CharacterMetadataController.cs
│
├── Models/
│   ├── Character.cs
│   ├── CharacterMappings.cs
│   ├── ClassType.cs
│   ├── FactionType.cs
│   ├── RaceType.cs
│   ├── Realm.cs
│   └── Auth (UserDto, TokenResponseDto, etc.)
│
├── Services/
│   ├── AuthService.cs / IAuthService.cs
│   ├── CharacterService.cs / ICharacterService.cs
│   └── CharacterMetadataService.cs / ICharacterMetadataService.cs
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
│       ├── CharacterTests.cs
│       └── CharacterMetadataTests.cs
```

## 🔐 API Endpoints

All endpoints are prefixed with `/api`.

### Auth

| Method | Endpoint              | Description              |
|--------|-----------------------|--------------------------|
| POST   | `/auth/register`      | Register new users       |
| POST   | `/auth/login`         | Authenticate and get JWT |
| POST   | `/auth/refresh-token` | Refresh access token     |

### Characters

| Method | Endpoint          | Description              |
|--------|-------------------|--------------------------|
| GET    | `/characters`     | Get all characters       |
| POST   | `/characters`     | Create new character     |
| PUT    | `/characters/{id}`| Update a character       |
| DELETE | `/characters/{id}`| Delete a character       |

### Metadata

| Method | Endpoint             | Description                    |
|--------|----------------------|--------------------------------|
| GET    | `/character-metadata/factions`  | Get faction types   |
| GET    | `/character-metadata/races`     | Get race types      |
| GET    | `/character-metadata/classes`   | Get class types     |
| GET    | `/character-metadata/realms`    | Get realms          |
| GET    | `/character-metadata/charactermappings`    | Get character mappings          |

## Character Restrictions

The API enforces the following rules when creating or updating characters:

## ⚔️ Faction-Race-Class Restrictions

The `CharacterService.cs` enforces rules to maintain fantasy-world consistency between a character’s race, class, and faction.

- **Faction** must be one of the allowed factions.
- Each **faction** has specific allowed **races**.
- Each **race** is restricted to certain **classes**.

These rules are enforced when creating or updating a character. If a character's attributes violate these constraints, the service returns a failure result, preventing invalid characters from being saved.

This helps maintain narrative and gameplay consistency in any game built on top of this API.

### Level Restrictions

- Character **Level** must be between `1` and `50`.

### Name Length Restrictions

- Character **Name** max length must not exceed `15` characters.

### Username Length Restrictions

- **Username** must not exceed `20` characters.

## 🛠 How to Run

1. Clone the repository
   ```bash
   git clone https://github.com/your-username/CharacterManagerApiTutorial.git
   cd CharacterManagerApiTutorial
   ```

2. Update `appsettings.json` with your SQL Server connection string

3. Run database migrations
   ```bash
   dotnet ef database update
   ```

4. Run the app
   ```bash
   dotnet run --project CharacterManagerApiTutorial
   ```

5. Run tests
   ```bash
   dotnet test
   ```

## 🧪 Test Coverage

- AuthController: `AuthControllerTests.cs`
- Services:
  - `AuthTests.cs`
  - `CharacterTests.cs`
  - `CharacterMetadataTests.cs`

## 📝 License

MIT — free for educational or commercial use.

## 👤 Author

**Cliff Nieman**  
GitHub: [@Zendrakk](https://github.com/Zendrakk) 
