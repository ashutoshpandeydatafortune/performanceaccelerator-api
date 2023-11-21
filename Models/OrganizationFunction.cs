namespace DF_EvolutionAPI.Models
{
    public class OrganizationFunction : BaseEntity
    {
        public string FunctionName { get; set; }
        public string Description { get; set; }
        public byte IsActive { get; set; }
        public byte IsDeleted { get; set; }
    }
}
