using System;

namespace DF_EvolutionAPI.Models
{
    public class QuarterDetails : BaseEntity
    {
        public int? QuarterYear { get; set; }
        public string QuarterName { get; set; }
        public string Description { get; set; }
        public string QuarterYearRange { get; set; }

        public byte? IsActive { get; set; }
        public byte? IsDeleted { get; set; }
        public int? StatusId { get; set; }
    }

    public class QuarterPeriod
    {
        public int QuarterId { get; set; }
        public string QuarterName { get; set; }
        public int QuarterYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
