using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models
{
    public class BugReportViewModel
    {
        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}