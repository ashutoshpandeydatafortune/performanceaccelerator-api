using System;

namespace DF_PA_API.Models.Response
{
    public class ResourceSelectedSkill
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public double ResourceExperience { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public int SkillId { get; set; }
    }
}
