using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Configuration
{
    public class DbOptions
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;
    }
}
