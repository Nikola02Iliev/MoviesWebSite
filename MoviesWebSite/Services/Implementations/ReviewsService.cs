using Microsoft.EntityFrameworkCore;
using MoviesWebSite.Context;
using MoviesWebSite.Models;
using MoviesWebSite.Services.Interfaces;

namespace MoviesWebSite.Services.Implementations
{
    public class ReviewsService : IReviewsService
    {
        private readonly AppDBContext _context;

        public ReviewsService(AppDBContext context)
        {
            _context = context;
        }
        public async Task AddReview(Review review)
        {

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReview(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Review> GetAllReviews()
        {
            var reviews = _context.Reviews
                .Include(r => r.Movie);

            return reviews;
        }

        public async Task<Review> GetReviewById(int? id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

            return review;
        }
    }
}
