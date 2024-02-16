using DF_EvolutionAPI.Models.Response;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DF_EvolutionAPI.Models
{
    public class Designation : BaseEntity
    {        
        [Key]
        public int DesignationId { get; set; }
        public int ReferenceId { get; set; }
        public string DesignationName { get; set; }  
        public int? IsActive { get; set; }
    }
}
