using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class RoleMapping
    {
        [Key]
        public int RoleMappingId { get; set; }
        public string RoleId { get; set; }
        public string ModuleName { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanDelete { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }
}
