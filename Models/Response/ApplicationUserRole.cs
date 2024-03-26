using Microsoft.AspNetCore.Identity;

namespace DF_EvolutionAPI.Models.Response
{
   public class ApplicationUserRole : IdentityUserRole<string>
    {
       public string ApplicationName { get; set; }
    }
}
