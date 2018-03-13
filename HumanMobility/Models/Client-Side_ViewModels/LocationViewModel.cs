using System;
using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models
{
    public class LocationViewModel
    {
        [Required]
        public DateTime SaveTime { get; set; }

        [Required]
        public DateTime DetectionTime { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public float Accuary { get; set; }

        [Required]
        public bool Error { get; set; }
    }
}
