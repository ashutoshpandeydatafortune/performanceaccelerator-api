using System;
using System.Collections.Generic;
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
        public int? BusinessUnitId { get; set; }
        [NotMapped]
        public string BusinessUnitName { get; set; }

        public List<PATemplateKra> AssignedKras { get; set; }
        public List<PATemplateDesignation> AssignedDesignations { get; set; }
    }

    public class UserKraResult
    {
        public int? UserId { get; set; }

        public int? KraId { get; set; }
    }

    //Represents a template associated with a specific function
    public class TemplateByFunction
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public int? FunctionId { get; set; }
        [NotMapped]
        public string FunctionName { get; set; }
        public byte? IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate     

    }

    //Represents a template associated with a specific business unit
    public class TemplateByBusinessUnit
    {
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public int? BusinessUnitId { get; set; }
        [NotMapped]
        public string BusinessUnitName { get; set; }
        public byte? IsActive { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; } // Nullable DateTime for UpdateDate  
    }
}