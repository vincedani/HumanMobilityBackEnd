using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanMobility.Models
{
    public class UserCredential
    {
        [Key]
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public bool HasAccelerometer { get; set; }

        [Required]
        public bool HasTemperatureSensor { get; set; }

        [Required]
        public string DeviceInfo { get; set; }

        [Required]
        public string Version { get; set; }


        public virtual ApplicationUser User { get; set; }
    }
}
