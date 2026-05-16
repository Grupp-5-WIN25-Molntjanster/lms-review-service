using LMS.ReviewService.Application.Interfaces;
using LMS.ReviewService.Domain.Entities;
using LMS.ReviewService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.ReviewService.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ReviewDbContext _context;

    public ReviewRepository(ReviewDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}