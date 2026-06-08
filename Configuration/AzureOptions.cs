using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Configuration
{
    public class AzureOptions
    {
        [Required]
        public string Instance { get; set; } = string.Empty;

        [Required]
        public string Domain { get; set; } = string.Empty;

        [Required]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public string CallbackPath { get; set; } = string.Empty;

        [Required]
        public string StorageConnectionString { get; set; } = string.Empty;
    }
}
