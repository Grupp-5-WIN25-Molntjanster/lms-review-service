using LMS.ReviewService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LMS.ReviewService.Domain.Interfaces;
using LMS.ReviewService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ReviewDatabase")));

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();