using MoviesWebSite.Models;

namespace MoviesWebSite.Services.Interfaces
{
    public interface IReviewsService
    {
        IQueryable<Review> GetAllReviews();
        Task AddReview(Review review);
        Task<Review> GetReviewById(int? id);
        Task DeleteReview(Review review);

    }
}
