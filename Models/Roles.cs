using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class Role
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }

        public int IsActive { get; set; }

        [NotMapped]
        public List<RoleMapping> RoleMappings { get; set; }
    }
}
