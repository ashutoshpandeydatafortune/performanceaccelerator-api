namespace DF_EvolutionAPI.Models
{
    public class Designation : BaseEntity
    {
        public int? DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int? IsActive { get; set; }
        public int? IsTimesheetApplies { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; }
    }
}
