using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class ProjectResource: BaseEntity_PRMS
    {
        public int ProjectResourceId { get; set; }
        public int? ProjectId { get; set; }
        public int? ResourceId { get; set; }
        public Boolean Billable { get; set; }
        public Boolean Shadow { get; set; }
        public int? PercentageAllocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        //public int? IsActive { get; set; }
        public int? BillingCycleId { get; set; }
        public int? CurrencyId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Rate { get; set; }
        public string ResourceRole { get; set; }

        public byte IsActive { get; set; }

        [NotMapped]
        public List<Project> ProjectList { get; set; }
    }
}
