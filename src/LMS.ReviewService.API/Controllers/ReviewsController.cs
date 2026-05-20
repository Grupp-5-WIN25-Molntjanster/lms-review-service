using LMS.ReviewService.Application.Reviews;
using LMS.ReviewService.Domain.Entities;
using LMS.ReviewService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        // Hämta recensioner från databasen baserat på kurs-ID
        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);

        // Mappa recensionerna till ReviewResponse-objekt
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
        // Hämta recensioner från databasen baserat på kurs-ID
        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);

        // Konvertera till lista för att kunna använda LINQ-funktioner flera gånger
        var reviewList = reviews.ToList();

        // Om det inte finns några recensioner, returnera en tom sammanfattning
        if (!reviewList.Any())
        {
            return Ok(new RatingSummaryResponse());
        }

        // Beräkna sammanfattningen av recensionerna
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
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Create(CreateReviewRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new
            {
                message = "Rating must be between 1 and 5."
            });

        // Hämta användarens ID och roll från JWT-token
        var userIdClaim = User.FindFirst("sub")?.Value;
        // Om du har en "role" claim i din JWT, kan du hämta den på samma sätt
        var role = User.FindFirst("role")?.Value;

        // Kontrollera om userIdClaim är null eller inte kan konverteras till Guid
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid or missing user id claim."
            });
        }

        // Kontrollera om användaren redan har recenserat kursen
        var review = new Review
        {
            Id = Guid.NewGuid(),
            CourseId = request.CourseId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Spara recensionen i databasen
        await _reviewRepository.AddAsync(review);
        // Spara ändringarna i databasen
        await _reviewRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByCourse),
            new { courseId = review.CourseId }, review);
    }

    // Denna endpoint används för att uppdatera en befintlig recension.
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, UpdateReviewRequest request)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review == null)
        {
            return NotFound();
        }

        // Endast administratörer eller den användare som skapade recensionen kan uppdatera den.
        var userIdClaim = User.FindFirst("sub")?.Value;
        var role = User.FindFirst("role")?.Value;

        // Kontrollera om användaren är administratör eller ägare av recensionen
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user token."
            });
        }
        // Om användaren inte är administratör och inte äger recensionen, returnera 401 Unauthorized
        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new
            {
                message = "Rating must be between 1 and 5."
            });

        // Kontrollera om användaren är administratör eller ägare av recensionen
        if (role != "Administrator" && review.UserId != userId)
        {
            return Unauthorized(new
            {
                message = "You are not the owner of this review."
            });
        }
        // Uppdatera recensionen
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