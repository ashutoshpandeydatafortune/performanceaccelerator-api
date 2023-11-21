using System;

namespace DF_EvolutionAPI.Models
{
    public class BaseEntity_PRMS
    {
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
