using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanMobility.Models
{
    public class Log : Common.EntityBase
    {
        [Required]
        [ForeignKey("Admin")]
        public string AdminId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Admin { get; set; }
    }
}
