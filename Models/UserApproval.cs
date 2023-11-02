namespace DF_EvolutionAPI.Models
{
    public class UserApproval  : BaseEntity
    {       
        public int ApprovalStatus { get; set; }
        public int AppraisalRange { get; set; }
        public int ApprovedBy { get; set; }
        public int RejectedBy { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int KRAId { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
    }
}
