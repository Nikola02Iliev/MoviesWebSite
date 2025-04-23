using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesWebSite.Context;
using MoviesWebSite.Models;
using MoviesWebSite.Paginator;
using MoviesWebSite.Services.Interfaces;
using MoviesWebSite.VMs;

namespace MoviesWebSite.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMoviesService _moviesService;
        private readonly IRatingsService _ratingsService;
        private readonly IReviewsService _reviewsService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(IMoviesService moviesService, IRatingsService ratingsService, IReviewsService reviewsService, IWebHostEnvironment webHostEnvironment)
        {
            _moviesService = moviesService;
            _ratingsService = ratingsService;
            _reviewsService = reviewsService;
            _webHostEnvironment = webHostEnvironment;
        }



        // GET: Movies
        public async Task<IActionResult> Index(int? pageNumber, string searchValue, string sortValue)
        {
            await _ratingsService.RecalculateAllAverageRatings();

            var movies = _moviesService.GetAllMovies();
            int pageSize = 6;

            if (!string.IsNullOrEmpty(searchValue))
            {
                movies = movies.Where(m => m.MovieName.Contains(searchValue));
                return View(await PaginatedList<Movie>.CreateAsync(movies.OrderByDescending(m => m.DatePublished).AsNoTracking(), pageNumber ?? 1, pageSize));

            }

            switch (sortValue)
            {
                case "descendingByDatePublished":
                    return View(await PaginatedList<Movie>.CreateAsync(movies.OrderByDescending(m => m.DatePublished).AsNoTracking(), pageNumber ?? 1, pageSize));
                case "ascendingByDatePublished":
                    return View(await PaginatedList<Movie>.CreateAsync(movies.OrderBy(m => m.DatePublished).AsNoTracking(), pageNumber ?? 1, pageSize));
                case "descendingByYearReleased":
                    return View(await PaginatedList<Movie>.CreateAsync(movies.OrderByDescending(m => m.YearReleased).AsNoTracking(), pageNumber ?? 1, pageSize));
                case "ascendingByYearReleased":
                    return View(await PaginatedList<Movie>.CreateAsync(movies.OrderBy(m => m.YearReleased).AsNoTracking(), pageNumber ?? 1, pageSize));
                default:
                    return View(await PaginatedList<Movie>.CreateAsync(movies.OrderByDescending(m => m.DatePublished).AsNoTracking(), pageNumber ?? 1, pageSize));

            }

        }

        public async Task<IActionResult> MyMovies(int? pageNumber)
        {
            var movies = _moviesService.GetAllMovies();
            int pageSize = 3;

            return View(await PaginatedList<Movie>.CreateAsync(movies.Where(m => m.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).OrderByDescending(m => m.DatePublished).AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        public async Task<IActionResult> MyReviews(int? pageNumber)
        {
            var reviews = _reviewsService.GetAllReviews();
            int pageSize = 6;

            return View(await PaginatedList<Review>.CreateAsync(reviews.Where(m => m.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).OrderByDescending(r => r.ReviewedDateTime).AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        public async Task<IActionResult> MyRatings(int? pageNumber)
        {
            var ratings = _ratingsService.GetAllRatings();
            int pageSize = 6;

            return View(await PaginatedList<Rating>.CreateAsync(ratings.Where(m => m.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).OrderByDescending(r => r.RatedDateTime).AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            var movie = await _moviesService.GetMovieById(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieVM movieVM)
        {
            if (movieVM.Image != null)
            {
                // Define the directory to save images
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                // Ensure the directory exists
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Generate a unique file name using GUID
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(movieVM.Image.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await movieVM.Image.CopyToAsync(fileStream);
                }

                // Get the relative path to save in the database
                string relativePath = Path.Combine("Images", fileName).Replace("\\", "/");

                // Create a new movie model with the relative path
                var movieModel = new Movie
                {
                    MovieName = movieVM.MovieName,
                    MovieDescription = movieVM.MovieDescription,
                    ImagePath = relativePath, // Use relative path with unique name
                    YearReleased = movieVM.YearReleased,
                    DatePublished = movieVM.DatePublished,
                    AverageRating = movieVM.AverageRating,
                    UserId = movieVM.UserId
                };

                // Add the movie to the database
                await _moviesService.AddMovie(movieModel);

                // Redirect to the index page
                return RedirectToAction("Index");
            }

            // If no image is provided, return the same view with the model
            return View(movieVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewVM reviewVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var reviewModel = new Review
                    {
                        ReviewTitle = reviewVM.ReviewTitle,
                        ReviewContent = reviewVM.ReviewContent,
                        ReviewedDateTime = reviewVM.ReviewedDateTime,
                        MovieId = reviewVM.MovieId,
                        UserId = reviewVM.UserId
                    };

                    await _reviewsService.AddReview(reviewModel);
                    return RedirectToAction("Details", new { id = reviewVM.MovieId });
                }

                var movie = await _moviesService.GetMovieById(reviewVM.MovieId);
                return View("Details", movie);
            }
            catch (DbUpdateException ex)
            {
                // Handle the InvalidOperationException
                // For example, you can log the error or display an error message
                return RedirectToAction("ReviewRestriction", "Errors");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(RatingVM ratingVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ratingModel = new Rating
                    {
                        RatingLevel = ratingVM.RatingLevel,
                        RatedDateTime = ratingVM.RatedDateTime,
                        MovieId = ratingVM.MovieId,
                        UserId = ratingVM.UserId
                    };

                    await _ratingsService.AddRating(ratingModel);
                    return RedirectToAction("Details", new { id = ratingVM.MovieId });
                }

                var movie = await _moviesService.GetMovieById(ratingVM.MovieId);
                return View("Details", movie);
            }
            catch (DbUpdateException ex)
            {
                // Handle the InvalidOperationException
                // For example, you can log the error or display an error message
                return RedirectToAction("RatingRestriction", "Errors");
            }
        }



        // GET: Movies/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var movie = await _moviesService.GetMovieById(id);
            if (movie == null)
            {
                return NotFound();
            }

            var updateMovieVM = new UpdateMovieVM
            {
                MovieId = movie.MovieId,
                MovieName = movie.MovieName,
                MovieDescription = movie.MovieDescription,
                YearReleased = movie.YearReleased,
                ImagePath = movie.ImagePath, // Set current image path
                UserId = movie.UserId
            };

            return View(updateMovieVM);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMovieVM updateMovieVM)
        {
            if (ModelState.IsValid)
            {
                var movie = await _moviesService.GetMovieById(updateMovieVM.MovieId);
                if (movie == null)
                {
                    return NotFound();
                }

                // Handle image upload if a new image is provided
                if (updateMovieVM.Image != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateMovieVM.Image.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateMovieVM.Image.CopyToAsync(fileStream);
                    }

                    string relativePath = Path.Combine("Images", fileName).Replace("\\", "/");
                    movie.ImagePath = relativePath; // Update image path
                }

                movie.MovieName = updateMovieVM.MovieName;
                movie.MovieDescription = updateMovieVM.MovieDescription;
                movie.YearReleased = updateMovieVM.YearReleased;
                // Do not update DatePublished
                

                await _moviesService.UpdateMovie(movie);

                return RedirectToAction("Index");
            }

            return View(updateMovieVM);
        }

        //        // GET: Movies/Delete/5
        //        public async Task<IActionResult> Delete(int? id)
        //        {
        //            if (id == null)
        //            {
        //                return NotFound();
        //            }

        //            var movie = await _context.Movies
        //                .Include(m => m.User)
        //                .FirstOrDefaultAsync(m => m.MovieId == id);
        //            if (movie == null)
        //            {
        //                return NotFound();
        //            }

        //            return View(movie);
        //        }

        // POST: Movies/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _moviesService.GetMovieById(id);
            await _moviesService.DeleteMovie(movie);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _ratingsService.GetRatingById(id);
            await _ratingsService.DeleteRating(rating);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]

        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _reviewsService.GetReviewById(id);
            await _reviewsService.DeleteReview(review);

            return RedirectToAction("Index");
        }

        //        private bool MovieExists(int id)
        //        {
        //            return _context.Movies.Any(e => e.MovieId == id);
        //        }
    }
}
