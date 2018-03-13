using System;
using System.ComponentModel.DataAnnotations;
using HumanMobility.Helpers;

namespace HumanMobility.Models
{
    public class ResearchViewModel
    {
        public DateTime From { get; set; } = DateTime.Today;

        public DateTime To { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "User")]
        public string UserName { get; set; }

        [Display(Name = "Export type")]
        public ExportType ExportType { get; set; } = ExportType.Database;

        public Fill Fill { get; set; } = Fill.LastData;
    }
}
