using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanMobility.Models
{
    public class Activity : Common.EntityBase
    {
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public DateTime SaveTime { get; set; }
        
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }


        public virtual ApplicationUser User { get; set; }
    }
}
