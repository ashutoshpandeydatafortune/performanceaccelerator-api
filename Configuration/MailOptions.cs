using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Configuration
{
    public class MailOptions
    {
        [Required]
        public string SMTP_HOST { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int SMTP_PORT { get; set; }

        [Required]
        public string SMTP_USERNAME { get; set; } = string.Empty;

        [Required]
        public string SMTP_PASSWORD { get; set; } = string.Empty;
    }
}
