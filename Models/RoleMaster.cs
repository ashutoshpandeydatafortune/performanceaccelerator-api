using DF_EvolutionAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_PA_API.Models
{
    public class RoleMaster
    {
        [Key]
        public string RoleId { get; set; }
        public string RoleName { get; set; }  
        public string Description {  get; set; }     
        public byte? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsAdmin { get; set; }

        [NotMapped]
        public List<RoleMapping> RoleMappings { get; set; }
    }
}
