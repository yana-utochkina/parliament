using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParliamentInfrastructure.Models;

namespace ParliamentInfrastructure
{
    public class IdentityContext : IdentityDbContext<DefaultUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}
