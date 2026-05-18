using LMS.ReviewService.Application.Reviews;
using LMS.ReviewService.Domain.Entities;
using LMS.ReviewService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.ReviewService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    // Detta är en enkel endpoint för att hämta alla recensioner för en specifik kurs.
    // Den tar emot ett courseId som parameter och returnerar en lista med recensioner i form av ReviewResponse-objekt.
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetByCourse(Guid courseId)
    {
        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);

        var response = reviews.Select(r => new ReviewResponse
        {
            Id = r.Id,
            CourseId = r.CourseId,
            UserId = r.UserId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAtUtc = r.CreatedAtUtc
        });

        return Ok(response);
    }

    // Denna endpoint används för att skapa en ny recension.
    // Den tar emot en CreateReviewRequest som innehåller information om kursen, användaren, betyget och kommentaren.
    [HttpPost]
    public async Task<ActionResult> Create(CreateReviewRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest("Rating must be between 1 and 5.");
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review);

        return CreatedAtAction(nameof(GetByCourse),
            new { courseId = review.CourseId }, review);
    }

    // Denna endpoint används för att uppdatera en befintlig recension.
    // Den tar emot ett id som parameter för att identifiera recensionen som ska uppdateras, samt en UpdateReviewRequest som innehåller de nya värdena för betyget och kommentaren.
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateReviewRequest request)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest("Rating must be between 1 and 5.");
        }

        review.Rating = request.Rating;
        review.Comment = request.Comment;

        await _reviewRepository.UpdateAsync(review);

        return NoContent();
    }

    // Denna endpoint används för att ta bort en recension baserat på dess ID.
    // Den tar emot ett id som parameter, hämtar recensionen från databasen och om den finns, tar bort den.
    // Om recensionen inte finns, returnerar den en NotFound-status.
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        await _reviewRepository.DeleteAsync(review);

        return NoContent();
    }
}