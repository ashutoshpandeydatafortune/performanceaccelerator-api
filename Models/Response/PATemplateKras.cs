using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models.Response
{
    public class PATemplateKras
    {
        [Key]
        public int TemplateKrasId { get; set; }
        public int TemplateId { get; set; }
        public int KraId { get; set; }
        public byte? IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate
    }
}
