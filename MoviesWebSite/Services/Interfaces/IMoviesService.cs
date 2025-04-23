using MoviesWebSite.Models;

namespace MoviesWebSite.Services.Interfaces
{
    public interface IMoviesService
    {
        IQueryable<Movie> GetAllMovies();
        Task<Movie> GetMovieById(int? id);
        Task AddMovie(Movie movie);
        Task DeleteMovie(Movie movie);
        Task UpdateMovie(Movie movie);
    }
}
