namespace DF_EvolutionAPI.Models
{
    public class Designation : BaseEntity
    {
        public int ReferenceId { get; set; }
        public int? DesignationId { get; set; }
        public string ReferenceName { get; set; }
        public string DesignationName { get; set; }
        public int? IsTimesheetApplies { get; set; }

        public int? IsActive { get; set; }
    }
}
