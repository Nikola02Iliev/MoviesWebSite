using Microsoft.AspNetCore.Identity;

namespace MoviesWebSite.Models
{
    public class AppUser : IdentityUser
    {
        public List<Review>? Reviews { get; set; }
        public List<Rating>? Ratings { get; set; }
        public List<Movie>? Movies { get; set; }
    }
}
