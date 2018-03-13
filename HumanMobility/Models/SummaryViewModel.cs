using System;
using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models
{
    public class SummaryViewModel
    {
        [Required]
        [Display(Name = "From")]
        public DateTime FromDate { get; set; } = DateTime.Today;
    }
}
