namespace LMS.ReviewService.Application.Reviews;

public class CreateReviewRequest
{
    public Guid CourseId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}