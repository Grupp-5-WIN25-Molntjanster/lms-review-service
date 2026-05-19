using LMS.ReviewService.Application.Reviews;
using LMS.ReviewService.Domain.Entities;
using LMS.ReviewService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.ReviewService.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    // Detta är en enkel endpoint för att hämta alla recensioner för en specifik kurs.
    [HttpGet("course/{courseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    // Denna endpoint används för att hämta en sammanfattning av recensionerna för en specifik kurs.
    [HttpGet("course/{courseId}/summary")]
    public async Task<ActionResult<RatingSummaryResponse>> GetRatingSummary(Guid courseId)
    {
        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);

        var reviewList = reviews.ToList();

        if (!reviewList.Any())
        {
            return Ok(new RatingSummaryResponse());
        }

        var summary = new RatingSummaryResponse
        {
            AverageRating = Math.Round(reviewList.Average(r => r.Rating), 1),

            TotalReviews = reviewList.Count,

            FiveStar = reviewList.Count(r => r.Rating == 5),

            FourStar = reviewList.Count(r => r.Rating == 4),

            ThreeStar = reviewList.Count(r => r.Rating == 3),

            TwoStar = reviewList.Count(r => r.Rating == 2),

            OneStar = reviewList.Count(r => r.Rating == 1)
        };

        return Ok(summary);
    }

    // Denna endpoint används för att skapa en ny recension.
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Create(CreateReviewRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new
            {
                message = "Rating must be between 1 and 5."
            });

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
        await _reviewRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByCourse),
            new { courseId = review.CourseId }, review);
    }

    // Denna endpoint används för att uppdatera en befintlig recension.
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, UpdateReviewRequest request)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new
            {
                message = "Rating must be between 1 and 5."
            });

        review.Rating = request.Rating;
        review.Comment = request.Comment;

        await _reviewRepository.UpdateAsync(review);
        await _reviewRepository.SaveChangesAsync();

        return NoContent();
    }

    // Denna endpoint används för att ta bort en recension baserat på dess ID.
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        await _reviewRepository.DeleteAsync(review);
        await _reviewRepository.SaveChangesAsync();

        return NoContent();
    }
}