# 📚 Library Management System – .NET 10 with DDD

A complete library management system built with .NET 10 following 
**Domain-Driven Design (DDD)** principles, MongoDB, and REST API. This project serves as a comprehensive learning resource for building production-ready applications using DDD architecture patterns.

## 🤔 What is Domain-Driven Design?

Domain-Driven Design is an approach to software development that focuses on creating a model that reflects the business domain. It emphasizes collaboration between technical and domain experts to create a shared understanding (Ubiquitous Language) and focuses on the core domain logic.

**Key Benefits:**

- **Better Organization**: Clear separation of concerns across layers
- **Business Alignment**: Code that reflects real business concepts
- **Maintainability**: Easier to modify and extend
- **Testability**: Domain logic isolated from infrastructure
- **Scalability**: Architecture that grows with your needs

## 🧱 Tech Stack

- **ASP.NET Core 10**: Web framework
- **MongoDB**: NoSQL database for document storage
- **xUnit**: Testing framework
- **Moq**: Mocking library for unit tests

## 🏗️ Project Architecture

```
LibraryManagement/
├── src/
│   ├── LibraryManagement.Core/          # Domain Layer (Entities, Value Objects, Interfaces)
│   ├── LibraryManagement.Application/   # Application Layer (Services, DTOs)
│   ├── LibraryManagement.Infrastructure/# Infrastructure Layer (Repositories, MongoDB)
│   └── LibraryManagement.API/           # Presentation Layer (Controllers, REST API)
└── tests/
    └── LibraryManagement.Tests/         # Unit Tests
```

### Architecture Layers

#### 1. 🎯 Domain Layer
The core of the application containing business logic and rules.

**Components:**
- **Entities**: Book, Member, Loan
- **Value Objects**: ISBN, Email
- **Repository Interfaces**: IBookRepository, IMemberRepository, ILoanRepository

**Principles:**
- No external dependencies
- Pure business logic only
- Framework-independent
- Entities protect their own invariants

#### 2. 🔄 Application Layer
Orchestrates data flow between the UI and domain.

**Components:**
- **Services**: BookService, MemberService, LoanService
- **DTOs**: Data Transfer Objects
- **Service Interfaces**: Application service contracts

**Principles:**
- No business logic
- Coordinates domain operations
- Transforms entities to DTOs and vice versa
- Manages transactions

#### 3. 🔧 Infrastructure Layer
Implements technical details and external resource communication.

**Components:**
- **Repositories**: Concrete implementations using MongoDB
- **Persistence**: MongoDbContext
- **External Services**: Third-party integrations (future)

**Principles:**
- Implements domain-defined interfaces
- Contains implementation details (ORM, APIs, etc.)
- Can be replaced without affecting domain

#### 4. 🌐 Presentation Layer
Exposes functionality through a REST API.

**Components:**
- **Controllers**: BooksController, MembersController, LoansController
- **Middlewares**: Error handling, logging
- **Request/Response DTOs**: API models

**Principles:**
- No business logic
- Validates user input
- Returns standardized responses
- Manages authentication/authorization (future)

## ✨ DDD Concepts Implemented

### Entities
Domain objects with unique identity and lifecycle:

**Book** - Represents a book in the system
- Properties: Id, Title, Author, ISBN, PublicationYear, Category, TotalCopies, AvailableCopies
- Behaviors: BorrowCopy(), ReturnCopy(), AddCopies(), CanBeBorrowed()
- Rules: Cannot lend without available copies

**Member** - Represents a library member
- Properties: Id, Name, Email, PhoneNumber, Status, BorrowedBookIds
- Behaviors: BorrowBook(), ReturnBook(), Suspend(), Reactivate(), CanBorrowBooks()
- Rules: Maximum of 5 borrowed books simultaneously

**Loan** - Represents a book loan
- Properties: Id, BookId, MemberId, LoanDate, DueDate, ReturnDate, Status, LateFee
- Behaviors: ReturnBook(), RenewLoan(), IsOverdue(), CalculateLateFee()
- Rules: 14-day loan period, $2.00/day late fee

### Value Objects
Immutable objects representing domain concepts:

**ISBN** - Validates and encapsulates ISBN-10 and ISBN-13 numbers
**Email** - Validates email format and normalizes to lowercase

### Repositories
Abstraction for data persistence with interfaces in Domain layer and implementations in Infrastructure layer.

### Domain Services
Business logic that doesn't belong to a specific entity, encapsulated in application services.

## 📋 Features

### Books
- ✅ Register new books
- 🔍 Search by title, author, or category
- ➕ Add copies
- ✔️ Check availability

### Members
- ✅ Register members
- 🚫 Suspend/reactivate members
- 📚 List borrowed books
- 📊 5 books per member limit

### Loans
- ✅ Create loans
- 📥 Return books
- 🔄 Renew loans
- 💰 Calculate late fees ($2.00/day)
- ⏰ 14-day default loan period

## 🚀 How to Run the Project

### Prerequisites
- .NET 10 SDK ([Download](https://dotnet.microsoft.com/download))
- MongoDB (local or Docker)

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/edumacielp/library-management.git
cd library-management
```

### 2️⃣ Configure MongoDB Connection

Update `appsettings.json` with your MongoDB connection string:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "DatabaseName": "LibraryManagement"
}
```

### 3️⃣ Restore Dependencies
```bash
dotnet restore
```

### 4️⃣ Run the API
```bash
cd src/LibraryManagement.API
dotnet run
```

The API will be available at `https://localhost:5001` (or your configured port).

### 🐳 Run MongoDB with Docker

```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

## 🧪 Running Tests

```bash
cd tests/LibraryManagement.Tests
dotnet test
```

## 📚 API Endpoints

### Books
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | List all books |
| GET | `/api/books/{id}` | Get book by ID |
| GET | `/api/books/search/title/{title}` | Search by title |
| GET | `/api/books/search/author/{author}` | Search by author |
| GET | `/api/books/category/{category}` | Search by category |
| GET | `/api/books/available` | List available books |
| POST | `/api/books` | Register new book |
| PUT | `/api/books/{id}` | Update book |
| POST | `/api/books/{id}/add-copies` | Add copies |
| DELETE | `/api/books/{id}` | Delete book |

### Members
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/members` | List all members |
| GET | `/api/members/{id}` | Get member by ID |
| GET | `/api/members/active` | List active members |
| GET | `/api/members/search/{name}` | Search by name |
| POST | `/api/members` | Register new member |
| PUT | `/api/members/{id}` | Update member |
| POST | `/api/members/{id}/suspend` | Suspend member |
| POST | `/api/members/{id}/reactivate` | Reactivate member |
| DELETE | `/api/members/{id}` | Delete member |

### Loans
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/loans` | List all loans |
| GET | `/api/loans/{id}` | Get loan by ID |
| GET | `/api/loans/active` | List active loans |
| GET | `/api/loans/overdue` | List overdue loans |
| GET | `/api/loans/member/{memberId}` | List member's loans |
| GET | `/api/loans/book/{bookId}` | List book's loans |
| POST | `/api/loans` | Create new loan |
| POST | `/api/loans/{id}/return` | Return book |
| POST | `/api/loans/{id}/renew` | Renew loan |

## 📝 API Usage Examples

### Register a Book
```bash
curl -X POST https://localhost:5001/api/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Clean Code",
    "author": "Robert C. Martin",
    "isbn": "978-0132350884",
    "publicationYear": 2008,
    "category": "Programming",
    "totalCopies": 5
  }'
```

### Create a Loan
```bash
curl -X POST https://localhost:5001/api/loans \
  -H "Content-Type: application/json" \
  -d '{
    "bookId": "book-id-here",
    "memberId": "member-id-here"
  }'
```

### Search Books by Author
```bash
curl -X GET https://localhost:5001/api/books/search/author/Robert%20C.%20Martin
```

## 🗄️ Database Schema

### Books Collection
```javascript
{
  "_id": "guid",
  "Title": "string",
  "Author": "string",
  "ISBN": {
    "Value": "string"
  },
  "PublicationYear": "int",
  "Category": "string",
  "TotalCopies": "int",
  "AvailableCopies": "int",
  "CreatedAt": "datetime",
  "UpdatedAt": "datetime"
}
```

### Members Collection
```javascript
{
  "_id": "guid",
  "Name": "string",
  "Email": {
    "Value": "string"
  },
  "PhoneNumber": "string",
  "MembershipDate": "datetime",
  "Status": "enum",
  "BorrowedBookIds": ["string"],
  "CreatedAt": "datetime",
  "UpdatedAt": "datetime"
}
```

### Loans Collection
```javascript
{
  "_id": "guid",
  "BookId": "string",
  "MemberId": "string",
  "LoanDate": "datetime",
  "DueDate": "datetime",
  "ReturnDate": "datetime",
  "Status": "enum",
  "LateFee": "decimal",
  "CreatedAt": "datetime",
  "UpdatedAt": "datetime"
}
```

## 📊 Data Flow

```
HTTP Client
    ↓
API Controller (Presentation)
    ↓
Application Service (Application)
    ↓
Core Service / Entity (Domain)
    ↓
Repository Interface (Domain)
    ↓
Repository Implementation (Infrastructure)
    ↓
MongoDB
```

## 🎯 Learning Objectives

This project teaches you:

### DDD Fundamentals
- **Layered Architecture**: Separation of concerns
- **Ubiquitous Language**: Consistent naming between domain and code
- **Aggregates**: Entities with identity and lifecycle
- **Value Objects**: Immutable domain concepts
- **Repositories**: Data persistence abstraction
- **Domain Services**: Business logic orchestration
- **Encapsulation**: Business rules protected within entities

### .NET Best Practices
- Dependency Injection
- Repository Pattern
- Service Pattern
- DTO Pattern
- Clean Architecture principles

### MongoDB Integration
- Document-based data modeling
- Repository pattern implementation
- Schema design for NoSQL

## 🛠️ Design Patterns Used

1. **Repository Pattern** - Abstracts data persistence
2. **Service Pattern** - Encapsulates application logic
3. **DTO Pattern** - Separates domain models from API
4. **Dependency Injection** - Manages dependencies
5. **Value Object Pattern** - Immutable domain concepts

## 🔮 Future Improvements

### Short Term
- [ ] Add structured logging
- [ ] Implement global error handling
- [ ] Add input validation with FluentValidation
- [ ] API documentation with Swagger/OpenAPI

### Medium Term
- [ ] Implement authentication/authorization (JWT)
- [ ] Add Domain Events
- [ ] Implement caching (Redis)
- [ ] Add rate limiting
- [ ] Integration tests

### Long Term
- [ ] Migrate to microservices architecture
- [ ] Implement Event Sourcing
- [ ] Add observability (OpenTelemetry)
- [ ] API Gateway implementation
- [ ] CQRS pattern

## 📖 Resources

- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Best Practices](https://learn.microsoft.com/en-us/aspnet/core/)
- [MongoDB .NET Driver](https://www.mongodb.com/docs/drivers/csharp/)

## 🤝 Contributing

Feel free to:

- Fork this repository
- Open issues for bugs or suggestions
- Submit pull requests with improvements
- Use this as a template for your own DDD projects

## 📝 License

This project is for educational purposes. Feel free to use it as a learning resource or starting point for your own DDD applications.

## 🎓 Author Notes

This project demonstrates a production-ready DDD architecture in .NET 10. The clear separation of layers (Domain/Application/Infrastructure/Presentation), proper use of DDD building blocks, and MongoDB integration make this a solid foundation for enterprise applications.

**Key Takeaways:**

- DDD provides excellent structure for complex domains
- Layered architecture improves maintainability
- Value Objects enforce domain rules
- Repository pattern abstracts persistence concerns
- MongoDB works great for DDD aggregates

Happy coding! 🚀✨

⭐ If this project helped you learn DDD, consider giving it a star!