using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CALMS.Modules
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { 
        
        }
        public DbSet<ApplicationMenu> Menus { get; set; }
        public DbSet<ApplicationMenuAssignment> MenuRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationMenuAssignment>().HasKey(c => new { c.ApplicationMenuId, c.IdentityRoleId });
        }
    }
    
}
