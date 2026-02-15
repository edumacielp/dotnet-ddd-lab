# 🏛️ System Architecture – Library Management

## Overview

This document describes the architecture of the Library Management System, 
implemented following **Domain-Driven Design (DDD)** principles with .NET 10 and MongoDB.

## 📐 Application Layers

### 1. Domain Layer (Core Business Logic)

**Responsibility**: Contains pure business logic and domain rules.

**Components:**
- **Entities**: Book, Member, Loan
- **Value Objects**: ISBN, Email
- **Repository Interfaces**: IBookRepository, IMemberRepository, ILoanRepository
- **Domain Events** (future): Domain occurrence events

**Principles:**
- ✅ No external dependencies
- ✅ Contains only business logic
- ✅ Framework and infrastructure independent
- ✅ Entities protect their own invariants
- ✅ Rich domain model with behavior

#### Entities in Detail

##### Book Entity
```csharp
Properties:
├── Id: Guid
├── Title: string
├── Author: string
├── ISBN: ISBN (Value Object)
├── PublicationYear: int
├── Category: string
├── TotalCopies: int
├── AvailableCopies: int
├── CreatedAt: DateTime
└── UpdatedAt: DateTime

Behaviors:
├── BorrowCopy() → Decrements available copies
├── ReturnCopy() → Increments available copies
├── AddCopies(int count) → Adds physical copies
└── CanBeBorrowed() → Validates borrowing eligibility

Business Rules:
├── Cannot borrow without available copies
├── Available copies cannot exceed total copies
└── Must have valid ISBN
```

##### Member Entity
```csharp
Properties:
├── Id: Guid
├── Name: string
├── Email: Email (Value Object)
├── PhoneNumber: string
├── MembershipDate: DateTime
├── Status: MemberStatus (Active/Suspended)
├── BorrowedBookIds: List<Guid>
├── CreatedAt: DateTime
└── UpdatedAt: DateTime

Behaviors:
├── BorrowBook(Guid bookId) → Adds book to borrowed list
├── ReturnBook(Guid bookId) → Removes book from borrowed list
├── Suspend() → Changes status to suspended
├── Reactivate() → Changes status to active
└── CanBorrowBooks() → Validates borrowing eligibility

Business Rules:
├── Maximum 5 books borrowed simultaneously
├── Cannot borrow when suspended
├── Must have valid email
└── Cannot borrow duplicate books
```

##### Loan Entity
```csharp
Properties:
├── Id: Guid
├── BookId: Guid
├── MemberId: Guid
├── LoanDate: DateTime
├── DueDate: DateTime
├── ReturnDate: DateTime?
├── Status: LoanStatus (Active/Returned/Overdue)
├── LateFee: decimal
├── CreatedAt: DateTime
└── UpdatedAt: DateTime

Behaviors:
├── ReturnBook() → Marks as returned, calculates fees
├── RenewLoan(int days) → Extends due date
├── IsOverdue() → Checks if past due date
└── CalculateLateFee() → Computes late fees

Business Rules:
├── 14-day default loan period
├── Late fee: $2.00 per day
├── Cannot renew overdue loans
└── Must return before renewal
```

#### Value Objects in Detail

##### ISBN Value Object
```csharp
Purpose: Validate and encapsulate ISBN numbers

Properties:
└── Value: string

Validation:
├── Accepts ISBN-10 format (10 digits)
├── Accepts ISBN-13 format (13 digits)
├── Removes hyphens and spaces
└── Validates checksum (future)

Example:
├── Valid: "978-0132350884" → "9780132350884"
├── Valid: "0132350882" → "0132350882"
└── Invalid: "12345" → throws exception
```

##### Email Value Object
```csharp
Purpose: Validate and normalize email addresses

Properties:
└── Value: string

Validation:
├── Checks valid email format
├── Normalizes to lowercase
└── Trims whitespace

Example:
├── "User@Example.COM  " → "user@example.com"
└── "invalid-email" → throws exception
```

### 2. Application Layer (Orchestration)

**Responsibility**: Orchestrates data flow between UI and domain.

**Components:**
- **Application Services**: BookService, MemberService, LoanService
- **DTOs (Data Transfer Objects)**: Input/Output models
- **Service Interfaces**: Application service contracts
- **Mappers**: Entity ↔ DTO transformations

**Principles:**
- ❌ No business logic
- ✅ Coordinates domain operations
- ✅ Transforms entities to DTOs
- ✅ Manages transactions
- ✅ Validates input data

#### Service Layer Architecture

```
BookService
├── GetAllBooksAsync() → List<BookDto>
├── GetBookByIdAsync(Guid id) → BookDto
├── SearchByTitleAsync(string title) → List<BookDto>
├── SearchByAuthorAsync(string author) → List<BookDto>
├── GetByCategoryAsync(string category) → List<BookDto>
├── GetAvailableBooksAsync() → List<BookDto>
├── CreateBookAsync(CreateBookDto dto) → BookDto
├── UpdateBookAsync(Guid id, UpdateBookDto dto) → BookDto
├── AddCopiesAsync(Guid id, int count) → BookDto
└── DeleteBookAsync(Guid id) → bool

MemberService
├── GetAllMembersAsync() → List<MemberDto>
├── GetMemberByIdAsync(Guid id) → MemberDto
├── GetActiveMembersAsync() → List<MemberDto>
├── SearchByNameAsync(string name) → List<MemberDto>
├── CreateMemberAsync(CreateMemberDto dto) → MemberDto
├── UpdateMemberAsync(Guid id, UpdateMemberDto dto) → MemberDto
├── SuspendMemberAsync(Guid id) → MemberDto
├── ReactivateMemberAsync(Guid id) → MemberDto
└── DeleteMemberAsync(Guid id) → bool

LoanService
├── GetAllLoansAsync() → List<LoanDto>
├── GetLoanByIdAsync(Guid id) → LoanDto
├── GetActiveLoansAsync() → List<LoanDto>
├── GetOverdueLoansAsync() → List<LoanDto>
├── GetMemberLoansAsync(Guid memberId) → List<LoanDto>
├── GetBookLoansAsync(Guid bookId) → List<LoanDto>
├── CreateLoanAsync(CreateLoanDto dto) → LoanDto
├── ReturnBookAsync(Guid loanId) → LoanDto
└── RenewLoanAsync(Guid loanId, int days) → LoanDto
```

### 3. Infrastructure Layer (Technical Implementation)

**Responsibility**: Implements technical details and external resource communication.

**Components:**
- **Repositories**: MongoDB implementations
- **Persistence**: MongoDbContext configuration
- **External Services** (future): Email, notifications, etc.
- **Configuration**: Database settings

**Principles:**
- ✅ Implements domain-defined interfaces
- ✅ Contains implementation details
- ✅ Interchangeable (can switch from MongoDB to SQL)
- ✅ Isolated from domain logic

#### MongoDB Schema Design

```javascript
// Books Collection
{
  "_id": ObjectId("..."),
  "Title": "Clean Code",
  "Author": "Robert C. Martin",
  "ISBN": {
    "Value": "9780132350884"
  },
  "PublicationYear": 2008,
  "Category": "Programming",
  "TotalCopies": 10,
  "AvailableCopies": 7,
  "CreatedAt": ISODate("2024-01-15T10:30:00Z"),
  "UpdatedAt": ISODate("2024-01-20T14:22:00Z")
}

// Members Collection
{
  "_id": ObjectId("..."),
  "Name": "John Doe",
  "Email": {
    "Value": "john.doe@example.com"
  },
  "PhoneNumber": "+1-555-0123",
  "MembershipDate": ISODate("2024-01-01T00:00:00Z"),
  "Status": 0, // 0 = Active, 1 = Suspended
  "BorrowedBookIds": [
    "guid-1",
    "guid-2",
    "guid-3"
  ],
  "CreatedAt": ISODate("2024-01-01T00:00:00Z"),
  "UpdatedAt": ISODate("2024-01-20T16:45:00Z")
}

// Loans Collection
{
  "_id": ObjectId("..."),
  "BookId": "guid-book-123",
  "MemberId": "guid-member-456",
  "LoanDate": ISODate("2024-01-15T00:00:00Z"),
  "DueDate": ISODate("2024-01-29T00:00:00Z"),
  "ReturnDate": null,
  "Status": 0, // 0 = Active, 1 = Returned, 2 = Overdue
  "LateFee": 0.0,
  "CreatedAt": ISODate("2024-01-15T10:00:00Z"),
  "UpdatedAt": ISODate("2024-01-15T10:00:00Z")
}
```

#### Repository Pattern Implementation

```csharp
// Interface (in Domain Layer)
public interface IBookRepository
{
    Task<Book> GetByIdAsync(Guid id);
    Task<IEnumerable<Book>> GetAllAsync();
    Task<IEnumerable<Book>> SearchByTitleAsync(string title);
    Task<IEnumerable<Book>> SearchByAuthorAsync(string author);
    Task<IEnumerable<Book>> GetByCategoryAsync(string category);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Guid id);
}

// Implementation (in Infrastructure Layer)
public class BookRepository : IBookRepository
{
    private readonly IMongoCollection<Book> _books;
    
    public BookRepository(IMongoDatabase database)
    {
        _books = database.GetCollection<Book>("Books");
    }
    
    // Implementation details using MongoDB driver
}
```

### 4. Presentation Layer (API)

**Responsibility**: Exposes functionality through REST API.

**Components:**
- **Controllers**: BooksController, MembersController, LoansController
- **Middlewares**: Error handling, logging, CORS
- **DTOs**: Request/Response models
- **Validators**: Input validation (future)

**Principles:**
- ❌ No business logic
- ✅ Validates user input
- ✅ Returns standardized responses
- ✅ HTTP-specific concerns only
- ✅ Authentication/Authorization (future)

#### API Response Standards

```csharp
// Success Response
{
  "success": true,
  "data": {
    "id": "guid",
    "title": "Clean Code",
    // ... other fields
  },
  "message": null,
  "errors": null
}

// Error Response
{
  "success": false,
  "data": null,
  "message": "Book not found",
  "errors": [
    {
      "field": "id",
      "message": "The specified book does not exist"
    }
  ]
}

// Validation Error Response
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": [
    {
      "field": "title",
      "message": "Title is required"
    },
    {
      "field": "isbn",
      "message": "Invalid ISBN format"
    }
  ]
}
```

## 🔄 Complete Data Flow Example

### Scenario: Creating a New Loan

```
1. HTTP Request
   POST /api/loans
   Body: { "bookId": "guid-1", "memberId": "guid-2" }
   
   ↓

2. LoansController (Presentation Layer)
   - Validates request model
   - Calls LoanService.CreateLoanAsync()
   
   ↓

3. LoanService (Application Layer)
   - Retrieves Book using IBookRepository
   - Retrieves Member using IMemberRepository
   - Validates business rules:
     * Book is available
     * Member can borrow books
     * Member doesn't have this book
   - Creates Loan entity
   - Calls domain methods:
     * book.BorrowCopy()
     * member.BorrowBook(bookId)
   - Persists changes via repositories
   
   ↓

4. Domain Layer
   - Book.BorrowCopy() validates and decrements AvailableCopies
   - Member.BorrowBook() validates and adds to BorrowedBookIds
   - Loan entity created with business rules applied
   
   ↓

5. Repository (Infrastructure Layer)
   - IBookRepository.UpdateAsync() → MongoDB update
   - IMemberRepository.UpdateAsync() → MongoDB update
   - ILoanRepository.AddAsync() → MongoDB insert
   
   ↓

6. MongoDB
   - Documents updated/inserted
   - Transaction completed
   
   ↓

7. Response Flow (back up)
   - Repository returns updated entities
   - Service maps to DTOs
   - Controller returns HTTP 201 Created
   - Client receives response
```

## 🎯 Architectural Decisions

### Why DDD?

**Advantages:**
1. **Clear Separation**: Each layer has distinct responsibilities
2. **Testability**: Business logic isolated = easy to test
3. **Maintainability**: Changes in one layer don't affect others
4. **Scalability**: Easy to add new features
5. **Ubiquitous Language**: Code reflects business domain
6. **Team Collaboration**: Domain experts and developers share vocabulary

**When to Use:**
- ✅ Complex business domains
- ✅ Long-term projects
- ✅ Multiple developers
- ✅ Evolving requirements

**When NOT to Use:**
- ❌ Simple CRUD applications
- ❌ Short-term projects
- ❌ Very small teams
- ❌ Well-defined, static requirements

### Why MongoDB?

**Advantages:**
1. **Schema Flexibility**: Easy to evolve the model
2. **Document Model**: Natural mapping for aggregates
3. **Performance**: Fast read operations
4. **Scalability**: Horizontal scaling support
5. **Developer Productivity**: Less boilerplate than relational DBs

**Trade-offs:**
- ❌ No built-in transactions across collections (requires sessions)
- ❌ Limited JOIN capabilities
- ❌ Data duplication for denormalization

**Alternatives Considered:**
- SQL Server (more rigid schema, better for complex queries)
- PostgreSQL (strong consistency, better for relational data)
- Entity Framework with SQL (type-safe, LINQ support)

### Design Patterns Applied

#### 1. Repository Pattern
**Purpose**: Abstract data access logic

```csharp
// Usage in Service
public class BookService
{
    private readonly IBookRepository _bookRepository;
    
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public async Task<BookDto> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return MapToDto(book);
    }
}
```

**Benefits:**
- ✅ Testability (easy to mock)
- ✅ Flexibility (swap implementations)
- ✅ Separation of concerns

#### 2. Service Pattern
**Purpose**: Encapsulate application logic

**Benefits:**
- ✅ Reusable business operations
- ✅ Transaction management
- ✅ Orchestration of multiple entities

#### 3. DTO Pattern
**Purpose**: Decouple API contracts from domain models

```csharp
// Domain Entity
public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public ISBN ISBN { get; private set; } // Value Object
    // ... methods and business logic
}

// API DTO
public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; } // Simple string
    // ... no business logic
}
```

**Benefits:**
- ✅ API stability (internal changes don't break clients)
- ✅ Security (expose only what's needed)
- ✅ Performance (optimize data transfer)

#### 4. Dependency Injection
**Purpose**: Manage dependencies and promote loose coupling

```csharp
// Startup configuration
services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IBookService, BookService>();
```

**Benefits:**
- ✅ Testability
- ✅ Flexibility
- ✅ Maintainability

## 🧪 Testing Strategy

### Test Pyramid

```
        /\
       /E2E\          ← Few (full system tests)
      /------\
     /  Integ  \      ← Some (repository, API tests)
    /----------\
   /    Unit     \    ← Many (domain, services tests)
  /--------------\
```

### Unit Tests (Current - ✅ Implemented)

**Scope**: Domain entities and value objects

```csharp
[Fact]
public void BorrowCopy_WithAvailableCopies_DecreasesCount()
{
    // Arrange
    var book = new Book("Title", "Author", new ISBN("9780132350884"), ...);
    
    // Act
    book.BorrowCopy();
    
    // Assert
    Assert.Equal(4, book.AvailableCopies);
}

[Fact]
public void Member_CannotBorrow_WhenHasFiveBooks()
{
    // Arrange
    var member = CreateMemberWithFiveBooks();
    
    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => 
        member.BorrowBook(Guid.NewGuid())
    );
}
```

**Coverage:**
- ✅ Book entity: 100%
- ✅ Member entity: 100%
- ✅ Loan entity: 100%
- ✅ ISBN value object: 100%
- ✅ Email value object: 100%

### Integration Tests (Future - 🔜 Planned)

**Scope**: Repositories, database operations

```csharp
[Fact]
public async Task BookRepository_GetByIdAsync_ReturnsCorrectBook()
{
    // Arrange - use test database
    var repository = new BookRepository(testDatabase);
    var bookId = await SeedTestBook();
    
    // Act
    var book = await repository.GetByIdAsync(bookId);
    
    // Assert
    Assert.NotNull(book);
    Assert.Equal("Test Title", book.Title);
}
```

### End-to-End Tests (Future - 🔜 Planned)

**Scope**: Complete API flows

```csharp
[Fact]
public async Task CreateLoan_CompleteFlow_ReturnsSuccess()
{
    // Arrange - setup test server
    var client = CreateTestClient();
    
    // Act
    var response = await client.PostAsync("/api/loans", loanRequest);
    
    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

## 🚀 Future Improvements Roadmap

### Phase 1: Foundation (Short Term - 1-2 months)
- [ ] **Logging**: Implement Serilog for structured logging
- [ ] **Error Handling**: Global exception middleware
- [ ] **Validation**: FluentValidation for input validation
- [ ] **API Documentation**: Swagger/OpenAPI integration
- [ ] **Health Checks**: Endpoint for monitoring
- [ ] **Integration Tests**: Repository and API tests

### Phase 2: Security & Performance (Medium Term - 3-6 months)
- [ ] **Authentication**: JWT-based authentication
- [ ] **Authorization**: Role-based access control (Admin, Librarian, Member)
- [ ] **Caching**: Redis for frequently accessed data
- [ ] **Rate Limiting**: Protect against abuse
- [ ] **Pagination**: Efficient data retrieval
- [ ] **Indexing**: MongoDB index optimization

### Phase 3: Advanced Features (Long Term - 6-12 months)
- [ ] **Domain Events**: Event-driven architecture
  ```csharp
  public class BookBorrowedEvent : IDomainEvent
  {
      public Guid BookId { get; }
      public Guid MemberId { get; }
      public DateTime OccurredOn { get; }
  }
  ```
- [ ] **Event Sourcing**: Audit trail for all operations
- [ ] **CQRS**: Separate read and write models
- [ ] **Notifications**: Email/SMS for due dates and overdues
- [ ] **Analytics**: Borrowing trends, popular books
- [ ] **Microservices**: Split into separate services
  - Books Service
  - Members Service
  - Loans Service
  - Notifications Service

### Phase 4: Enterprise (Future)
- [ ] **API Gateway**: Centralized entry point
- [ ] **Service Discovery**: Dynamic service location
- [ ] **Distributed Tracing**: OpenTelemetry integration
- [ ] **Message Queue**: RabbitMQ/Kafka for async operations
- [ ] **Multi-tenancy**: Support multiple libraries
- [ ] **Containerization**: Docker & Kubernetes deployment

## 📊 Monitoring & Observability (Future)

### Logging Strategy
```csharp
// Structured logging example
_logger.LogInformation(
    "Book borrowed: {BookId} by {MemberId} at {Time}",
    bookId, memberId, DateTime.UtcNow
);
```

### Metrics to Track
- Request rate per endpoint
- Average response time
- Error rate
- Database query performance
- Active loans count
- Overdue loans count
- Member registration rate

### Health Checks
```
GET /health
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "memory": "Healthy",
    "disk": "Healthy"
  }
}
```

## 🔐 Security Considerations (Future)

### Authentication Flow
```
User Login
    ↓
API validates credentials
    ↓
Generate JWT token
    ↓
Client stores token
    ↓
All requests include token in header
    ↓
API validates token on each request
```

### Authorization Levels
- **Admin**: Full system access
- **Librarian**: Manage books, members, loans
- **Member**: View own loans, search books

### Data Protection
- Encrypt sensitive data at rest
- Use HTTPS for all communications
- Implement CORS policies
- Sanitize inputs to prevent injection
- Rate limiting to prevent DDoS

## 📚 Learning Resources

### Books
- **Domain-Driven Design** - Eric Evans (The Blue Book)
- **Implementing Domain-Driven Design** - Vaughn Vernon (The Red Book)
- **Clean Architecture** - Robert C. Martin
- **Patterns of Enterprise Application Architecture** - Martin Fowler

### Online Resources
- [Microsoft - DDD and CQRS patterns](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)
- [MongoDB .NET Driver Documentation](https://www.mongodb.com/docs/drivers/csharp/)
- [DDD Community](https://github.com/ddd-crew)

### Related Patterns
- **CQRS** (Command Query Responsibility Segregation)
- **Event Sourcing**
- **Hexagonal Architecture** (Ports & Adapters)
- **Onion Architecture**
- **Clean Architecture**

## 🤝 Contributing Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation for public APIs
- Keep methods small and focused

### Pull Request Process
1. Create feature branch
2. Implement changes with tests
3. Update documentation
4. Submit PR with description
5. Address review comments

### Commit Message Format
```
feat: add book reservation feature
fix: correct late fee calculation
docs: update architecture diagram
test: add member service tests
refactor: simplify loan creation logic
```

## 📝 License

MIT License - Free for educational and commercial use.

---

**Remember**: Good architecture is about making the right trade-offs for your specific context. This project demonstrates DDD principles, but adapt it to your needs!

🚀 Happy architecting!