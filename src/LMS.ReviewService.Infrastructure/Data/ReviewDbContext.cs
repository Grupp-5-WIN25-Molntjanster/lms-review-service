using LMS.ReviewService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.ReviewService.Infrastructure.Data;

public class ReviewDbContext : DbContext
{
    public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
        : base(options)
    {
    }

    public DbSet<Review> Reviews => Set<Review>();
}