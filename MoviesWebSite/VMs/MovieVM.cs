using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MoviesWebSite.Models;

namespace MoviesWebSite.VMs
{
    public class MovieVM
    {
        
        [Required]
        public string MovieName { get; set; }

        [Required]
        public string MovieDescription { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        public int YearReleased { get; set; }

        public DateTime DatePublished { get; set; } = DateTime.Now;

        public double AverageRating { get; set; }

        
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public string GetFormattedDatePublished()
        {
            return DatePublished.ToString("dd-MM-yyyy HH:mm");
        }
    }
}
