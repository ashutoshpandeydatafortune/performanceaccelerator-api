using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    [Table("PA_Skills")]
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public ICollection<SubSkill> SubSkills { get; set; }
        public ICollection<ResourceSkill> ResourceSkills { get; set; }

    }

    public class SkillDetails
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SubSkill> subSkills { get; set; }
    }
}
