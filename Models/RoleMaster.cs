using System;
using System.ComponentModel.DataAnnotations;

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
    }
}
