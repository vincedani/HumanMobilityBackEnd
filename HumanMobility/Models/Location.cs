using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanMobility.Models
{
    public class Location : Common.EntityBase
    {
        [Required]
        [ForeignKey("User")]
        [Index("UserAndSaveTime_Index", 1, IsUnique = true)]
        public string UserId { get; set; }

        [Required]
        [Index("UserAndSaveTime_Index", 2, IsUnique = true)]
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


        public virtual ApplicationUser User { get; set; }
    }
}
