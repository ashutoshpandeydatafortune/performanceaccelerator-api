using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models.Response
{
    public class PATemplateDesignation
    {
        [Key]
        public int TemplateDesignationId { get; set; }
        public int TemplateId { get; set; }
        public int DesignationId { get; set; }
        public bool IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate
    }
}
