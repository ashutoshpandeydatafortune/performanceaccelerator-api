namespace DF_EvolutionAPI.Models
{
    public class RoleMapping : BaseEntity
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
    }
}
