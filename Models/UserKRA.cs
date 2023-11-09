using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class UserKRA : BaseEntity
    {
        public string DeveloperComment { get; set; }
        public string ManagerComment { get; set; }
        public int? DeveloperRating { get; set; }
        public int? ManagerRating { get; set; }
        public int? FinalRating { get; set; }
        public string FinalComment { get; set; }
        public double Score { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public int KRAId { get; set; }
        public int? QuarterId { get; set; }
        public int? ApprovedBy { get; set; }
        public int? RejectedBy { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public int? AppraisalRange { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
    }

    public class UserKRADetails : BaseEntity
    {
        [NotMapped]
        public int Id { get; set; }
        public string DeveloperComment { get; set; }
        public string ManagerComment { get; set; }
        public int? DeveloperRating { get; set; }
        public int? ManagerRating { get; set; }
        public int? FinalRating { get; set; }
        public string FinalComment { get; set; }
        public double Score { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Reason { get; set; }
        public int UserId { get; set; }
        public int KRAId { get; set; }
        public int? QuarterId { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? WeightageId { get; set; }
        public int Weightage { get; set; }
    }
}
