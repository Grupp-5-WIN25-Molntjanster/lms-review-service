using System.Net;
using System.Net.Http.Json;
using LMS.ReviewService.Application.Reviews;
using Xunit;

namespace LMS.ReviewService.IntegrationTests;

public class ReviewsControllerTests
    : IClassFixture<TestApplicationFactory>
{
    private readonly HttpClient _client;

    public ReviewsControllerTests(TestApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateReview_ThenGetReviews_ReturnsCreatedReview()
    {
        var courseId = Guid.NewGuid();

        var request = new CreateReviewRequest
        {
            CourseId = courseId,
            UserId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Excellent course"
        };

        var postResponse = await _client.PostAsJsonAsync(
            "/api/reviews",
            request);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var reviews = await _client.GetFromJsonAsync<List<ReviewResponse>>(
            $"/api/reviews/course/{courseId}");

        Assert.NotNull(reviews);

        Assert.Single(reviews!);

        Assert.Equal(5, reviews[0].Rating);

        Assert.Equal("Excellent course", reviews[0].Comment);
    }

    [Fact]
    public async Task UpdateReview_ReturnsNoContent()
    {
        var courseId = Guid.NewGuid();

        var createRequest = new CreateReviewRequest
        {
            CourseId = courseId,
            UserId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Original review"
        };

        var postResponse = await _client.PostAsJsonAsync("/api/reviews", createRequest);
        var createdReview = await postResponse.Content.ReadFromJsonAsync<ReviewResponse>();

        var updateRequest = new UpdateReviewRequest
        {
            Rating = 4,
            Comment = "Updated review"
        };

        var putResponse = await _client.PutAsJsonAsync(
            $"/api/reviews/{createdReview!.Id}",
            updateRequest);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
    }

    [Fact]
    public async Task GetRatingSummary_ReturnsCorrectSummary()
    {
        var courseId = Guid.NewGuid();

        await _client.PostAsJsonAsync("/api/reviews", new CreateReviewRequest
        {
            CourseId = courseId,
            UserId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Great"
        });

        await _client.PostAsJsonAsync("/api/reviews", new CreateReviewRequest
        {
            CourseId = courseId,
            UserId = Guid.NewGuid(),
            Rating = 3,
            Comment = "Okay"
        });

        var summary = await _client.GetFromJsonAsync<RatingSummaryResponse>(
            $"/api/reviews/course/{courseId}/summary");

        Assert.NotNull(summary);
        Assert.Equal(2, summary!.TotalReviews);
        Assert.Equal(4.0, summary.AverageRating);
        Assert.Equal(1, summary.FiveStar);
        Assert.Equal(1, summary.ThreeStar);
    }

    [Fact]
    public async Task DeleteReview_ReturnsNoContent()
    {
        var createRequest = new CreateReviewRequest
        {
            CourseId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Review to delete"
        };

        var postResponse = await _client.PostAsJsonAsync("/api/reviews", createRequest);
        var createdReview = await postResponse.Content.ReadFromJsonAsync<ReviewResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/reviews/{createdReview!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}