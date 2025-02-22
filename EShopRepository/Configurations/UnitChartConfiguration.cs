using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class UnitChartConfiguration : IEntityTypeConfiguration<UnitChart>
    {
        public UnitChartConfiguration()
        { 

        }

        public void Configure(EntityTypeBuilder<UnitChart> builder)
        {
            builder.Property(co => co.UnitTypeID).IsRequired(true);
            builder.Property(co => co.ProductID).IsRequired(true);
            builder.Property(co => co.Quantity).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(f => f.UnitChartName).HasMaxLength(100).IsUnicode(false).IsRequired(true);
            builder.HasIndex(t => new { t.UnitTypeID, t.ProductID }).HasDatabaseName("IX_UnitChartUnique").IsUnique(true);
        }
    }
}
