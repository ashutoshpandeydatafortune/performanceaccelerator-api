using System.Collections.Generic;

namespace DF_EvolutionAPI.Models
{
    public class UserNotificationData
    {
        public string UserName { get; set; }
        public string ManagerName { get; set; }
        public string SrManagerName { get; set; }
        public string Email { get; set; }
        public bool IsForSrManager { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<UserKRA> userKRAs { get; set; }
}
}
