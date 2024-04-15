using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    [Table("PA_SubSkills")]
    public class SubSkill
    {
        public int SubSkillId { get; set; }
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
