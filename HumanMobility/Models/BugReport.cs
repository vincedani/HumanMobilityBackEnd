using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanMobility.Models
{
    public class BugReport : Common.EntityBase
    {
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Message { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}