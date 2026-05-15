namespace LMS.ReviewService.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid UserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}