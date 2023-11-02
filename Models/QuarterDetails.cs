using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class QuarterDetails: BaseEntity
    {
        public string QuarterName { get; set; }
        public int? QuarterYear { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public int? StatusId { get; set; }
    }
}
