using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class Project
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? NumberOfResorces { get; set; }
        public string Duration { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyBillingRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? WeeklyBillingRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? HourlyBillingRate { get; set; }
        public int? ReviewCycleId { get; set; }
        public int? ProjectManagerId { get; set; }
        public int? ProjectLeadId { get; set; }
        public int? ClientId { get; set; }
        public int? ProjectTypeId { get; set; }
        public int? ProjectStatusId { get; set; }
        public int? ProjectSubTypeId { get; set; }
        public int? CategoryId { get; set; }
        public byte IsActive { get; set; }

        [NotMapped]

        public List<ProjectResource> ProjectResourceList = new List<ProjectResource>();
    }
}
