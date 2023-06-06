using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
	

    public class ElectricityInfoConfiguration : IEntityTypeConfiguration<ElectricityInfo>
    {
        public void Configure(EntityTypeBuilder<ElectricityInfo> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Network).HasMaxLength(80);
        }
    }
}

