using LMS.ReviewService.Domain.Entities;

namespace LMS.ReviewService.Application.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetByCourseIdAsync(Guid courseId);

    Task<Review?> GetByIdAsync(Guid id);

    Task AddAsync(Review review);

    Task UpdateAsync(Review review);

    Task DeleteAsync(Review review);

    Task SaveChangesAsync();
}