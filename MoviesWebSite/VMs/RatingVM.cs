using MoviesWebSite.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebSite.VMs
{
    public class RatingVM
    {
        [Required]
        [Range(0, 6)]
        public int RatingLevel { get; set; }

        public DateTime RatedDateTime { get; set; } = DateTime.Now;

        public int? MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie? Movie { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public string GetFormattedRatedDateTime()
        {
            return RatedDateTime.ToString("dd-MM-yyyy HH:mm");
        }
    }
}
