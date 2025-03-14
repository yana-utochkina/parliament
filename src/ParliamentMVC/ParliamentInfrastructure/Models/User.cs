using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace ParliamentInfrastructure.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string University { get; set; }
        public string Faculty {  get; set; }
    }
}
