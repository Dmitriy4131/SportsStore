using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportsStore.Models
{
    public class AppIdentityDbContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<Microsoft.AspNetCore.Identity.IdentityUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
             : base(options) { }
    }
}
