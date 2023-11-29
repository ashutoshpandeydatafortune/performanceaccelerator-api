using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class Role
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }

        public int IsActive { get; set; }
    }
}
