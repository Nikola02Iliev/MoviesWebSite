using MoviesWebSite.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesWebSite.VMs
{
    public class UpdateMovieVM
    {
        [Key]
        public int MovieId { get; set; }

        [Required]
        public string MovieName { get; set; }

        [Required]
        public string MovieDescription { get; set; }

        public IFormFile? Image { get; set; } // Make image optional

        [Required]
        public int YearReleased { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public string? ImagePath { get; set; } // Store current image path
    }
}
