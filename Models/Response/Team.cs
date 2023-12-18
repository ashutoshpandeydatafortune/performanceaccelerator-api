namespace DF_EvolutionAPI.Models.Response
{
    public class Team
    {
        public string EmailId { get; set; } 
        public int ResourceId { get; set; }
        public int? ReportingTo { get; set; }
        public string EmployeeId { get; set; }
        public int? DesignationId { get; set; }
        public string PrimarySkill { get; set; }
        public string ResourceName { get; set; }
        public string DesignationName { get; set; }
        public int? ManagerId { get; set; }
    }
}
