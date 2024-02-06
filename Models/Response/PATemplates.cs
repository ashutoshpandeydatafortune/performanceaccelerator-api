using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models.Response
{
    public class PATemplates
    {
        [Key]
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate
    }
}
