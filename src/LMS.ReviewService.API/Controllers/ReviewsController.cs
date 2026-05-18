using LMS.ReviewService.Application.Interfaces;
using LMS.ReviewService.Application.Reviews;
using LMS.ReviewService.Domain.Entities;
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
}