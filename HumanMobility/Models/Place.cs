using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanMobility.Models
{
    public class Place : Common.EntityBase
    {
        [Required]
        [ForeignKey("User")]
        [Index("UserAndLocation_Index", 1, IsUnique = true)]
        public string UserId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Index("UserAndLocation_Index", 3, IsUnique = true)]
        public double Latitude { get; set; }

        [Required]
        [Index("UserAndLocation_Index", 4, IsUnique = true)]
        public double Longitude { get; set; }

        [Required]
        public int Radius { get; set; }


        public virtual ApplicationUser User { get; set; }
    }
}
