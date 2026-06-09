using System.Collections.Generic;

namespace DF_EvolutionAPI.Models
{
    public class UserNotificationData
    {
        public string UserName { get; set; }
        public string ManagerName { get; set; }
        public string SrManagerName { get; set; }
        public string Email { get; set; }
        public string UserEmail { get; set; }
        public string ManagerEmail { get; set; }
        public string SrManagerEmail { get; set; }
        public string RejectionReason { get; set; }
        public string RejectedByManagerName { get; set; }
        public bool IsRejection { get; set; }
        public bool IsApprovalComplete { get; set; }
        public bool IsForSrManager { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<UserKRA> userKRAs { get; set; }
    }
}
