using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models
{
    public class UserInfoViewModel
    {
        [Required]
        public bool HasAccelerometer { get; set; }

        [Required]
        public bool HasTemperatureSensor { get; set; }

        [Required]
        public string DeviceInfo { get; set; }

        [Required]
        public string Version { get; set; }
    }
}