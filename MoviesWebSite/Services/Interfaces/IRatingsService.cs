using MoviesWebSite.Models;

namespace MoviesWebSite.Services.Interfaces
{
    public interface IRatingsService
    {
        IQueryable<Rating> GetAllRatings();
        Task AddRating(Rating rating);

        Task RecalculateAllAverageRatings();

        Task DeleteRating(Rating rating);

        Task<Rating> GetRatingById(int? id);

    }
}
