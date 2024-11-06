using System;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Models

{
    public class ResourceSkill
    {
        public int ResourceSkillId { get; set; }
        public int? SkillId { get; set; }
        public int? SubSkillId { get; set; }
        public int? ResourceId { get; set; }
        public double? Experience { get; set; }
        public double? SkillExperience { get; set; }
        public double? SubSkillExperience { get; set; }
        public string SkillVersion {  get; set; }
        public string SkillDescription {  get; set; }
        public string SubSkillVersion {  get; set; }
        public string SubSkillDescription {  get; set; }
        
        public byte? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

    }

    public class ResourceSkillRequestModel
    {
        public int ResourceId { get; set; }
        public byte? IsActive { get; set; }
        public int CreateBy { get; set; }
        public List<SkillCategory> SkillCategories { get; set; }
    }

    public class SkillCategory
    {
        public List<SkillModel> Skills { get; set; }
    }

    public class FetchResourceSkill
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public double? TotalYears { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public List<SkillModel> Skills { get; set; }
    }

    public class FetchResourceCategorySkills
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public List<CategorySkillModel> CategoryWiseSkills { get; set; }
    }

    public class CategorySkillModel
    {
        public string CategoryName { get; set; }
        public List<SkillModel> Skills { get; set; }
    }

    public class SubSkillModel
    {
        public int? SubSkillId { get; set; }
        public int? SkillId { get; set; }        
        public string SubSkillName { get; set; }
        public double? SubSkillExperience { get; set; }
        public string SubSkillVersion { get; set; }
        public string SubSkillDescription { get; set; }

    }

    public class  SkillModel
    {
        public int SkillId { get; set; }   
        public string SkillName { get; set; }
      
        public double? SkillExperience { get; set; }
        public string SkillVersion { get; set; }
        public string SkillDescription { get; set; }

        public List<SubSkillModel> SubSkills { get; set; }
     
    }

   
}

