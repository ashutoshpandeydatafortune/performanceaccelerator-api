using System;
using System.ComponentModel.DataAnnotations;
namespace DF_PA_API.Models
{
    public class DesignatedRole
    {
        [Key]
        public int DesignatedRoleId { get; set; }
        public string DesignatedRoleName { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public byte? IsActive { get; set; }
    }    
}
