using Microsoft.EntityFrameworkCore;
using MoviesWebSite.Context;
using MoviesWebSite.Models;
using MoviesWebSite.Services.Interfaces;

namespace MoviesWebSite.Services.Implementations
{
    public class MoviesService : IMoviesService
    {
        private readonly AppDBContext _context;
        

        public MoviesService(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMovie(Movie movie)
        {

            var movieId = movie.MovieId;
            var movieReviews = await _context.Reviews.Where(r => r.MovieId == movieId).ToListAsync();
            var movieRatings = await _context.Ratings.Where(r => r.MovieId == movieId).ToListAsync();

            _context.Reviews.RemoveRange(movieReviews);
            _context.Ratings.RemoveRange(movieRatings);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Movie> GetAllMovies()
        {
            var movies = _context.Movies
                .Include(m => m.User);

            return movies;
        }

        public async Task<Movie> GetMovieById(int? id)
        {
            var movie = await _context.Movies
                .Include(m => m.User)
                .Include(m => m.Ratings)
                    .ThenInclude(rating => rating.User)
                .Include(m => m.Reviews)
                    .ThenInclude(review => review.User)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            return movie;
        }

        public async Task UpdateMovie(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}
