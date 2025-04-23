using MoviesWebSite.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesWebSite.VMs
{
    public class ReviewVM
    {
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
