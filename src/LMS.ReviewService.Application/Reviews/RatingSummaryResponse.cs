namespace LMS.ReviewService.Application.Reviews;

public class RatingSummaryResponse
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> StarDistribution { get; set; } = new();
}