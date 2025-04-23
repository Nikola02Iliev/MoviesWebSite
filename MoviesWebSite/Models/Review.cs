using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace MoviesWebSite.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string ReviewTitle { get; set; }

        [Required]
        public string ReviewContent { get; set; }

        public DateTime ReviewedDateTime { get; set; } = DateTime.Now;

        public int? MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie? Movie { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }


        public string GetFormattedReviewedDateTime()
        {
            return ReviewedDateTime.ToString("dd-MM-yyyy HH:mm");
        }
    }
}
