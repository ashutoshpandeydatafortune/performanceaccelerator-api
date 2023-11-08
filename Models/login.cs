using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class UserAuthModel
    {
        public string Username { get; set; }
        public string Key { get; set; }


        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Photo { get; set; }
        public string AccountType { get; set; }
        public string Action { get; set; }

        public int LogInUserID { get; set; }
        public string TimeZone { get; set; }
    }
}
