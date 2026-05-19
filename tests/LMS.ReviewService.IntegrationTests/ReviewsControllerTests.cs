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
        // Arrange
        var courseId = Guid.NewGuid();

        var request = new CreateReviewRequest
        {
            CourseId = courseId,
            UserId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Excellent course"
        };

        // Act - POST
        var postResponse = await _client.PostAsJsonAsync(
            "/api/reviews",
            request);

        // Assert POST
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        // Act - GET
        var reviews = await _client.GetFromJsonAsync<List<ReviewResponse>>(
            $"/api/reviews/course/{courseId}");

        // Assert GET
        Assert.NotNull(reviews);

        Assert.Single(reviews!);

        Assert.Equal(5, reviews[0].Rating);

        Assert.Equal("Excellent course", reviews[0].Comment);
    }
}