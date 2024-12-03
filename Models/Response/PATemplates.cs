using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models.Response
{
    [Table("PA_Templates")]
    public class PATemplate
    {
        [Key]
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte? IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate
        //For adding function name
        public int? FunctionId { get; set; }
        [NotMapped]
        public string FunctionName { get; set; }

        

        public  List<PATemplateKra> AssignedKras { get; set; }
        public List<PATemplateDesignation> AssignedDesignations { get; set; }

        public class UserKraResult
        {
            public int? UserId { get; set; }
            public int? KraId { get; set; }
        }


    }
}