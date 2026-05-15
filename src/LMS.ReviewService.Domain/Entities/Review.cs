using System.ComponentModel.DataAnnotations;

namespace LMS.ReviewService.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    [Required]
    public Guid CourseId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(500)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}