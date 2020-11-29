using BBDom.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BBDom.Data
{
    public class BBDomDbContext : IdentityDbContext
    {

        public DbSet<KnxState> KnxStates { get; set; }
        public DbSet<KnxGroup> KnxGroups { get; set; }

        public BBDomDbContext(DbContextOptions<BBDomDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<KnxState>()
                .HasIndex(b => b.Address)
                .IsUnique(false);

        }

        public BBDomDbContext() : base()
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=bb_dom.db");
        }

    }
}
