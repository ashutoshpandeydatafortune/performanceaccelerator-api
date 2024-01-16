using System.Collections.Generic;

namespace DF_EvolutionAPI.Models
{
    public class UserNotificationData
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public List<Notification> Notifications { get; set; }
}
}
