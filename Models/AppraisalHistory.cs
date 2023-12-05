using System;

namespace DF_EvolutionAPI.Models
{
    public class AppraisalHistory : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime LastAppraisalDate { get; set; }
        public int Percentage { get; set; }
        public int LastAppraisal { get; set; }
        // public int IsActive { get; set; }
         public bool IsActive { get; set; }
        //public int IsDeleted { get; set; }
        public bool IsDeleted { get; set; }
    }
}
