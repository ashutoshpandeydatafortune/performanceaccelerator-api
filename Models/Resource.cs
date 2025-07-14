using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class Resource : BaseEntity_PRMS
    {

        public int ResourceId { get; set; }
        public int? FunctionId { get; set; }
        public string EmployeeId { get; set; }
        public string ResourceName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoin { get; set; }

        [Required]
        public string EmailId { get; set; }


        #region Address
        public string Address { get; set; }
        public string CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Zip { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        public string AlternetNumber { get; set; }

        #endregion

        #region User Mapping

        public int? DesignationId { get; set; }
        public int? TechCategoryId { get; set; }
        public int? StatusId { get; set; }
        public int? ReportingTo { get; set; }
        public int? DesignatedRoleId { get; set; }

        #endregion

        #region Skills and Experience
        public string Primaryskill { get; set; }
        public string Secondaryskill { get; set; }
        public string Strengths { get; set; }
        public double? TotalYears { get; set; }
        public double? TenureInMonths { get; set; }
        public double? TenureInYears { get; set; }
        public int? YearBucket { get; set; }
        public List<ResourceSkill> ResourceSkills { get; set; }

        #endregion

        public byte IsActive { get; set; }

        [NotMapped]
        public string Function { get; set; }
        [NotMapped]
        public string Designation { get; set; }

        [NotMapped]
        public List<ProjectResource> ResourceProjectList { get; set; }

        [NotMapped]
        public List<Project> ProjectList { get; set; }

        [NotMapped]
        public List<Client> ClientList { get; set; }

        [NotMapped]
        public List<BusinessUnit> BusinessUnits { get; set; }

        [NotMapped]
        public string ReporterName { get; set; }

        [NotMapped]
        public string DesignationName { get; set; }

        [NotMapped]
        public List<AssignedSpecialKRA> SpecialKRAs { get; set; }
    }

    public class AssignedSpecialKRA
    {
        public int KRAId { get; set; }
        public string KraName { get; set; }
    }

    public class ResourceEvaluation
    {
        public int? ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string QuarterId { get; set; }
        public string QuarterName { get; set; }
    }

    public class ResourceEvaluationResponse
    {
        public List<ResourceEvaluation> ResourceEvaluationList { get; set; }
        public int totalCount { get; set; }
    }

    public class ResourceReportee
    {
        public int ResourceId { get; set; }       
        public string ResourceName { get; set; }
        public string DesignationName { get; set; }
        public int? DesignationId { get; set; }

    }

    public class ResourceProjectAssignment
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        [NotMapped]
        public DateTime? quarterEndDate { get; set; }
    }


}
