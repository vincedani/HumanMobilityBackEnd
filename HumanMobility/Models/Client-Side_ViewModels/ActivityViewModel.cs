using System;
using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models
{
    public class ActivityViewModel
    {
        // ReSharper disable once InconsistentNaming
        public long ID { get; set; }

        public string UserId { get; set; }

        [Required]
        public DateTime SaveTime { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }
    }
}