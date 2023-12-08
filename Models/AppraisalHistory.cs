using System;

namespace DF_EvolutionAPI.Models
{
    public class AppraisalHistory : BaseEntity
    {
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public int QuarterId { get; set; }
        public int Percentage { get; set; }
        public int LastAppraisal { get; set; }
        public DateTime LastAppraisalDate { get; set; }

        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
    }
}
