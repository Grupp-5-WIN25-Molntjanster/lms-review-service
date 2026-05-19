# LMS Review Service
## Owner: Joachim Persson
### Purpose
Handles course reviews and ratings.

### Architecture
Clean Architecture with ASP.NET Core Web API.
Projects included:
- LMS.ReviewService.API
- LMS.ReviewService.Application
- LMS.ReviewService.Domain
- LMS.ReviewService.Infrastructure
- LMS.ReviewService.IntegrationTests

### Tech Stack
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- xUnit Integration Tests
- YARP Gateway Integration

### Features
- Create review
- Get reviews by course
- Update review
- Delete review
- Rating summary
- Swagger documentation
- Integration tests

### Setup
1. Clone repository
2. Run migrations (bash)
   - dotnet ef database update \
   - project src/LMS.ReviewService.Infrastructure \
   - startup-project src/LMS.ReviewService.API
4. Start API (bash)
   dotnet run --project src/LMS.ReviewService.API

### Run Tests
dotnet test (bash)

### API Endpoints
#### | Method || Endpoint || Description |
- | GET |`/api/reviews/course/{courseId}`| Get reviews by course |
- | GET |`/api/reviews/course/{courseId}/summary`| Get rating summary |
- | POST |`/api/reviews`| Create review |
- | PUT |`/api/reviews/{id}`| Update review |
- | DEL |`/api/reviews/{id}`| Delete review |

### API Documentation
Swagger available at: http://localhost:5292/swagger

### Gateway Integration
This service is designed to be consumed through the YARP Gateway.
