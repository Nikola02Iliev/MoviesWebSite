using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MoviesWebSite.VMs;

namespace MoviesWebSite.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required]
        public string MovieName { get; set; }

        [Required]
        public string MovieDescription { get; set; }

        [Required]
        public string ImagePath { get; set; }

        [Required]
        public int YearReleased { get; set; }
        public DateTime DatePublished { get; set; } = DateTime.Now;

        public double AverageRating { get; set; }

        public List<Review>? Reviews { get; set; }
        public List<Rating>? Ratings { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        [NotMapped]
        public ReviewVM? ReviewVM { get; set; }

        [NotMapped]
        public RatingVM? RatingVM { get; set; }

        public string GetFormattedDatePublished()
        {
            return DatePublished.ToString("dd-MM-yyyy HH:mm");
        }
    }
}
