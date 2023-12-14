namespace DF_EvolutionAPI.Models
{
    public class KRALibrary : BaseEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? Entity { get; set; }
        public int? Entity2 { get; set; }
        public int? ApprovedBy { get; set; }
        public int? RejectedBy { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public int? IsSpecial { get; set; }
        public int? IsDefault { get; set; }
        public int? WeightageId { get; set; }
        public int? IsActive { get; set; }
        public int? IsDeleted { get; set; }
        public int Weightage { get; set; }

    }
}
