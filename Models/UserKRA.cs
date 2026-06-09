using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class UserKRA : BaseEntity
    {
        public string DeveloperComment { get; set; }
        public string ManagerComment { get; set; }
        public double? DeveloperRating { get; set; }
        public double? ManagerRating { get; set; }
        public double? FinalRating { get; set; }
        public string FinalComment { get; set; }
        public double Score { get; set; }
        public int? Status { get; set; }
        public int? UserId { get; set; }
        public int? KRAId { get; set; }
        public int? QuarterId { get; set; }
        public int? ApprovedBy { get; set; }
        public int? RejectedBy { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        [NotMapped]
        public string KRAName { get; set; }
        public int? AppraisalRange { get; set; }
        public byte? IsActive { get; set; }
        public byte? IsDeleted { get; set; }
        [NotMapped]
        public bool? isUpdated { get; set; }
        public byte? IsApproved { get; set; }
       
    }

    public class UserKRADetails : BaseEntity
    {
        public string DeveloperComment { get; set; }
        public string ManagerComment { get; set; }
        public double? DeveloperRating { get; set; }
        public double? ManagerRating { get; set; }
        public double? FinalRating { get; set; }
        public string FinalComment { get; set; }
        public double? Score { get; set; }
        public int? Status { get; set; }        
        public string Reason { get; set; }
        public int? UserId { get; set; }
        public int? KRAId { get; set; }
        public int? QuarterId { get; set; }
        public int? RejectedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public string RejectedByName { get; set; }
        public string QuarterName { get; set; }
        public int? QuarterYear { get; set; }
        public byte? IsActive { get; set; }
        public string KRAName { get; set; }
        public string KRADisplayName { get; set; }
        public string Description { get; set; }
        public int? WeightageId { get; set; }
        public int? Weightage { get; set; }
        public int? IsSpecial { get; set; }
        public bool? IsDescriptionRequired { get; set; }
        public int? MinimumRatingForDescription { get; set; }        
        public byte? IsApproved { get; set; }
        public string UserAchievement { get; set; }
        public string ManagerQuartelyComment { get; set; }
    }
    public class UpdateUserKRARequest
    {
        public List<UserKRA> UserKRAModel { get; set; }
        public string UserAchievement { get; set; }
        public string ManagerQuartelyComment { get; set; }
    }

    public class AssignedKras
    {
        public string KRAName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? Weightage { get; set; }
    }
  
}
