# DevFlow - Developer Project Management Platform

A full-stack project management application built with .NET 8 and Clean Architecture principles, featuring real-time notifications, caching optimization, and comprehensive API documentation.

## 🚀 Features

### Core Functionality
- **User Authentication & Authorization** - JWT-based secure authentication
- **Project Management** - Create, update, and organize development projects
- **Task Management** - Track tasks with priorities, statuses, and due dates
- **Code Snippets** - Store and search reusable code snippets with syntax highlighting
- **Analytics Dashboard** - Real-time project and task completion metrics

### Advanced Features
- **Real-Time Notifications** - SignalR WebSockets for instant task updates
- **Performance Caching** - In-memory caching with smart invalidation (50x faster analytics)
- **Rate Limiting** - API protection (30 requests/min, 100 requests/hour)
- **Search & Filtering** - Advanced query capabilities across all entities

## 🏗️ Architecture

Built using **Clean Architecture** principles:
```
┌─────────────────────────────────────────────┐
│           Presentation Layer (API)          │
│  Controllers, Hubs, Middleware, Services    │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Application Layer                   │
│  Services, DTOs, Interfaces                 │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│       Infrastructure Layer                  │
│  Repositories, DbContext, Caching           │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│           Domain Layer                      │
│  Entities, Enums, Business Rules            │
└─────────────────────────────────────────────┘
```

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Service Layer Pattern** - Business logic separation
- **Cache-Aside Pattern** - Performance optimization
- **Dependency Injection** - Loose coupling

## 🛠️ Tech Stack

### Backend
- **.NET 8** - Latest .NET framework
- **Entity Framework Core** - ORM and database management
- **SQL Server** - Relational database
- **SignalR** - Real-time WebSocket communication
- **JWT** - Secure authentication tokens

### Libraries & Packages
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `Microsoft.EntityFrameworkCore.SqlServer` - Database provider
- `AspNetCoreRateLimit` - API rate limiting
- `Microsoft.Extensions.Caching.Memory` - In-memory caching

## 📦 Project Structure
```
DevFlow/
├── src/
│   ├── DevFlow.API/              # Presentation layer
│   │   ├── Controllers/          # API endpoints
│   │   ├── Hubs/                 # SignalR hubs
│   │   └── Services/             # Infrastructure services
│   ├── DevFlow.Application/      # Business logic
│   │   ├── Services/             # Application services
│   │   ├── Interfaces/           # Service & repository interfaces
│   │   └── DTOs/                 # Data transfer objects
│   ├── DevFlow.Infrastructure/   # Data access
│   │   ├── Repositories/         # Repository implementations
│   │   └── Data/                 # DbContext, migrations
│   └── DevFlow.Domain/           # Core domain
│       ├── Entities/             # Domain models
│       └── Enums/                # Domain enumerations
└── README.md
```

## 🚦 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or Developer Edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
```bash
   git clone https://github.com/yourusername/DevFlow.git
   cd DevFlow
```

2. **Update connection string**
   
   Edit `src/DevFlow.API/appsettings.json`:
```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=DevFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
```

3. **Update JWT secret**
   
   In `appsettings.json`:
```json
   "JwtSettings": {
     "SecretKey": "your-secure-secret-key-at-least-32-characters-long"
   }
```

4. **Run database migrations**
```bash
   cd src/DevFlow.API
   dotnet ef database update
```

5. **Run the application**
```bash
   dotnet run
```

6. **Access Swagger UI**
   
   Navigate to: `https://localhost:5111/swagger`

## 📚 API Documentation

### Authentication

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com"
  }
}
```

### Projects

#### Create Project
```http
POST /api/projects
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "My Project",
  "description": "Project description"
}
```

#### Get All Projects
```http
GET /api/projects
Authorization: Bearer {token}
```

### Tasks

#### Create Task
```http
POST /api/projects/{projectId}/tasks
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Implement feature",
  "description": "Feature description",
  "priority": 2,
  "dueDate": "2026-03-25T00:00:00Z"
}
```

#### Update Task
```http
PUT /api/projects/{projectId}/tasks/{taskId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Updated title",
  "description": "Updated description",
  "status": 1,
  "priority": 3,
  "dueDate": "2026-03-30T00:00:00Z"
}
```

### Real-Time Notifications

Connect to SignalR hub:
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5111/hubs/tasks", {
        accessTokenFactory: () => yourJWTToken
    })
    .build();

// Listen for task events
connection.on("TaskCreated", (task) => {
    console.log("New task created:", task);
});

connection.on("TaskUpdated", (task) => {
    console.log("Task updated:", task);
});

connection.on("TaskDeleted", (taskId) => {
    console.log("Task deleted:", taskId);
});

// Start connection
await connection.start();

// Join project room
await connection.invoke("JoinProject", projectId);
```

## ⚡ Performance Features

### Caching Strategy
- **Pattern**: Cache-Aside (Lazy Loading)
- **Storage**: In-Memory Cache (IMemoryCache)
- **TTL**: 5 minutes for analytics
- **Invalidation**: Explicit on data changes
- **Result**: 50x faster analytics queries (50ms → 1ms)

### Rate Limiting
- **30 requests per minute** per IP
- **100 requests per hour** per IP
- Returns `429 Too Many Requests` when exceeded

## 🔐 Security Features

- **JWT Authentication** - Secure token-based auth
- **Password Hashing** - BCrypt password encryption
- **Authorization** - Role-based access control
- **Rate Limiting** - Prevents API abuse
- **CORS** - Configured for trusted origins

## 📊 Database Schema

### Core Entities
- **Users** - Application users
- **Projects** - User projects
- **Tasks** - Project tasks with status tracking
- **Snippets** - Code snippets with language tags

### Relationships
- User → Projects (1:N)
- Project → Tasks (1:N)
- User → Snippets (1:N)

## 🧪 Testing

Run tests:
```bash
dotnet test
```

## 🚀 Deployment

### Prerequisites
- Azure App Service or equivalent
- Azure SQL Database or SQL Server
- Configure environment variables

### Environment Variables
```
ConnectionStrings__DefaultConnection=<your-production-db-connection>
JwtSettings__SecretKey=<your-production-secret>
```

## 📈 Future Enhancements

- [ ] Frontend (React/Angular)
- [ ] Docker containerization
- [ ] Redis distributed caching
- [ ] Azure deployment
- [ ] Comprehensive unit tests
- [ ] Integration tests
- [ ] CI/CD pipeline

## 👨‍💻 Author

**Your Name**
- GitHub: @dhritinarsaria (https://github.com/dhritinarsaria)
- LinkedIn: Dhriti Narsaria (https://www.linkedin.com/in/dhriti-narsaria/)
- Email: dhritinarsaria@gmail.com



## 🙏 Acknowledgments

- Built as part of interview preparation
- Implements industry best practices and design patterns
- Focused on clean architecture, scalability, and performance

---

⭐ **Star this repo if you found it helpful!**