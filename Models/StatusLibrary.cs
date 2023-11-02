using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class StatusLibrary : BaseEntity
    {
        public string StatusName { get; set; }
        public string StatusType { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }

    }
}
