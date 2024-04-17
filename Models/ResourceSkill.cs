using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
    public class ResourceSkill
    {
        [Key]
        public int ResourceSkillId { get; set; }
        public int? SkillId { get; set; }
        public int? SubSkillId { get; set; }
        public int? ResourceId { get; set; }
        public double? Experience { get; set; }
        public byte? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    //public class ResourceSkillRequestModel
    //{
    //    public int ResourceSkillId { get; set; }
    //    public int? SkillId { get; set; }
    //    public List<SubSkillModel> SubSkills { get; set; }
    //    public int? ResourceId { get; set; }
    //    public double? Experience { get; set; }
    //    public byte? IsActive { get; set; }
    //    public int CreateBy { get; set; }
       

    //}

    //public class SubSkillModel
    //{
    //    public int SubSkillId { get; set; }
    //}
}
