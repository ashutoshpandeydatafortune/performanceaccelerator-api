using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DF_EvolutionAPI.Models.Response
{
    public class PATemplateDesignation
    {
        [Key]
        [Column("TemplateDesignationId")]
        public int TemplateDesignationId { get; set; }

        [ForeignKey("PATemplate")]
        public int TemplateId { get; set; }        
        public byte? IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate

        [JsonIgnore]
        public PATemplate PATemplate { get; set; }

        [ForeignKey("Designation")]
        public int DesignationId { get; set; }
        public Designation Designation { get; set; }
    }

    public class PATtemplateDesignationList
    {
        public int TemplateId { get; set; }
        public int CreateBy { get; set; }
        public List<int> DesignationIds { get; set; }
       


    }
}
