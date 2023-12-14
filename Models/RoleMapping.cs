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

        public byte? CanRead { get; set; }
        public byte? CanWrite { get; set; }
        public byte? CanDelete { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public byte? IsActive { get; set; }
    }
}
