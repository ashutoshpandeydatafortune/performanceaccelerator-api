namespace DF_EvolutionAPI.Models
{
    public class SubmissionStatus : BaseEntity
    {
        public int StatusId { get; set; }
        public string SubmissionName { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
    }
}
