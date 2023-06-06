using Core.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ElectricityInfoConfiguration());

            builder.Entity<ElectricityInfo>().ToTable("ElectricityInfos");
        }

        public virtual DbSet<ElectricityInfo> ElectricityInfos { get; set; }
    }
}

