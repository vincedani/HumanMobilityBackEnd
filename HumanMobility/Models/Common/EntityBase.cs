using System.ComponentModel.DataAnnotations;

namespace HumanMobility.Models.Common
{
    public abstract class EntityBase
    {
        [Key]
        [Required]
        // ReSharper disable once InconsistentNaming
        public long ID { get; set; }
    }
}
