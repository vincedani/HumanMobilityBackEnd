using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models
{
    public class PlaceViewModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public int Radius { get; set; }
    }
}