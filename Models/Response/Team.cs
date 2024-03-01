using System.Collections.Generic;

namespace DF_EvolutionAPI.Models.Response
{
    public class Team
    {
        public string EmailId { get; set; } 
        public int ResourceId { get; set; }
        public int? ReportingTo { get; set; }
        public string EmployeeId { get; set; }
        public int? DesignationId { get; set; }
        public string PrimarySkill { get; set; }
        public string ResourceName { get; set; }
        public string DesignationName { get; set; }
        public double? AverageScoreYear { get; set; }
        public double? AverageScoreCurrent { get; set; }

    }

    public class TeamDetails
    {
        public string EmailId { get; set; }
        public int ResourceId { get; set; }
        public int EmployeeId { get; set; }
        public string ResourceName { get; set; }
        public string DesignationName { get; set; }
        public double? Experience { get; set; }
        public double? AverageScoreYear { get; set; }
        public double? AverageScoreCurrent { get; set; }
        public int PendingEvaluation { get; set; }
        public int? ReportingTo { get; set; }
      

    }
    public class UserKRARatingLists

    {
        // public int? QuarterYear { get; set; }
        public string QuarterYearRange { get; set; }
       // public string? QuarterName { get; set; }  
        public double Rating { get; set; }
        public double Score { get; set; }
       

    }
}
