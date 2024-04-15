using System;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Models
{
    public class ResourceSkill
    {
       public int ResourceSkillId { get; set; }
    public int? SkillId { get; set; }
    //public List<SubSkillRequest> SubSkillId { get; set; }
    public int? ResourceId { get; set; }
    public float? Experience { get; set; }
    public byte? IsActive { get; set; }
    public int? CreateBy { get; set; }
    public int? UpdateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    }

    public class SubSkillRequest
    {
        public int? SubSkillId { get; set; }
    }
}
