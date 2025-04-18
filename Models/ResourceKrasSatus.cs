﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace DF_PA_API.Models
{
    public class ResourceKrasSatus
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        //public string Designation { get; set; }
        public string DesignatedRole { get; set; }
        public int Completed { get; set; }  
        public int Pending { get; set; }  
        public List<KraQuarter> Kras { get; set; }        
        [NotMapped]
        public string ReportingToName { get; set; }
        [NotMapped]
        public string ManagerReportingToName { get; set; }
    }

    public class KraQuarter
    {
        public string Quarter { get; set; } 
        public string QuarterName {  get; set; }
        
        public List<KraRating> Ratings { get; set; }
    }

    public class KraRating
    {
        public string KraName { get; set; }       
        public double? DeveloperRating { get; set; }
        public double? ManagerRating { get; set; }
        public double? FinalRating { get; set; }
        public int? RejectedBy { get; set; }
        public byte? IsApproved { get; set; }
    }
    
    public class FunctionsDesignations
    {
        public int FunctionId { get; set; }
        public int DesignationId { get; set; }
        public int DesignatedRoleId { get; set; }
        public string FunctionName { get; set; }
        public string DesignationName { get; set; }
        public string DesignatedRoleName { get; set; }
    }
    public class ReportingToName
    {
        [NotMapped]
        public string ManagerName { get; set; }
    }
}
