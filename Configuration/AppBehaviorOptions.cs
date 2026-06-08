using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Configuration
{
    public class AppBehaviorOptions
    {
        [Required]
        public string[] NoMailDesignation { get; set; } = System.Array.Empty<string>();

        [Required]
        public string[] Origins { get; set; } = System.Array.Empty<string>();
    }
}
