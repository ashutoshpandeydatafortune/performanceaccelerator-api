using System;
namespace DF_PA_API.Models
{
    public class UserQuarterlyAchievement
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? QuarterId { get; set; }
        public string UserAchievement { get; set; }
        public string ManagerQuartelyComment { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public byte? IsActive { get; set; }
    }
}
