namespace DF_EvolutionAPI.Models
{
    public class Designation : BaseEntity
    {
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int? IsActive { get; set; }
    }
}
