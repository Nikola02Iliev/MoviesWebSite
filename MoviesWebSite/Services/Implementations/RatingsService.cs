using Microsoft.EntityFrameworkCore;
using MoviesWebSite.Context;
using MoviesWebSite.Models;
using MoviesWebSite.Services.Interfaces;

namespace MoviesWebSite.Services.Implementations
{
    public class RatingsService : IRatingsService
    {
        private readonly AppDBContext _context;

        public RatingsService(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddRating(Rating rating)
        {

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            // Update the average rating for the movie
            var movie = await _context.Movies
                .Include(m => m.Ratings)
                .FirstOrDefaultAsync(m => m.MovieId == rating.MovieId);


            movie.AverageRating = movie.Ratings.Average(r => r.RatingLevel);
            await _context.SaveChangesAsync();


        }

        public async Task DeleteRating(Rating rating)
        {
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Rating> GetAllRatings()
        {
            var ratings = _context.Ratings
                .Include(r => r.Movie);

            return ratings;
        }

        public async Task<Rating> GetRatingById(int? id)
        {
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.RatingId == id);

            return rating;
            
        }

        public async Task RecalculateAllAverageRatings()
        {
            var movies = await _context.Movies
                .Include(m => m.Ratings)
                .ToListAsync();

            foreach (var movie in movies)
            {
                movie.AverageRating = movie.Ratings.Any() ? movie.Ratings.Average(r => r.RatingLevel) : 0;
            }

            await _context.SaveChangesAsync();
        }

        
    }
}
