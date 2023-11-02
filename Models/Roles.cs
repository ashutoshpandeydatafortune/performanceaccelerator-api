using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class Roles : BaseEntity
    {
        [Required]
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
           
    }
}
