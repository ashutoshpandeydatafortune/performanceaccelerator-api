using System.Collections.Generic;

namespace DF_EvolutionAPI.Models
{
    public class SearchSkill
    {
        public string SearchKey { get; set; }
        public List<int> SkillIds { get; set; }
        public List<int> SubSkillIds { get; set; }
    }
}
