using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
            builder.Property(x => x.Number).IsRequired().HasMaxLength(32);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.BranchId).IsRequired();
            builder.Property(x => x.BranchName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Total).HasColumnType("decimal(18,2)");
            builder.Property(x => x.IsCancelled).IsRequired();

            builder.HasMany<SaleItem>("_items")
             .WithOne()
             .HasForeignKey("SaleId")
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Number).IsUnique();
        }
    }
}
