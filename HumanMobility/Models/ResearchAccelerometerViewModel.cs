using System;
using System.ComponentModel.DataAnnotations;
using DataAnalyzer.Entities;

namespace HumanMobility.Models
{
    public class ResearchAccelerometerViewModel
    {
        [Required]
        [Display(Name = "User")]
        public string _UserName { get; set; }

        [Display(Name = "From")]
        public DateTime _From { get; set; } = DateTime.Today;

        [Display(Name = "To")]
        public DateTime _To { get; set; } = DateTime.Now;

        [Display(Name = "Activity calc.")]
        public AcceptableActivityOptions _ActivityOptions { get; set; } = AcceptableActivityOptions.EnergyExpenditure;

        [Display(Name = "Export")]
        public AcceptableExportOptions _ExportOptions { get; set; } = AcceptableExportOptions.Chart;
    }
}
