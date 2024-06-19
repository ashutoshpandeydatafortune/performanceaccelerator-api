using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class KRAWeightage : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int WeightageValue { get; set; }

        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
    }
}
