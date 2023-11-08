using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DF_EvolutionAPI.Models.Response
{
    public class LoginResponse
    {
        public string Id { get; set; }
        public int ResourceId { get; set; }
        public string UserName { get; set; }
        public int ReferenceId { get; set; }
        public int ReportingToId { get; set; }
        public IList<string> Roles { get; set; }
        public string ResourceName { get; set; }
        
        public string Token { get; set; }
        public IdentityRole Role { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
