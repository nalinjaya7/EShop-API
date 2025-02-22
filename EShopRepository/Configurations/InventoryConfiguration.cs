using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public InventoryConfiguration()
        {           

        }

        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.Property(co => co.BatchID).IsRequired(true); 
            builder.Property(co => co.ProductID).IsRequired(true);
            builder.Property(co => co.UnitChartID).IsRequired(true);
            builder.Property(co => co.Code).HasMaxLength(15).IsRequired(true);
            builder.Property(co => co.SellingPrice).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(co => co.PurchasePrice).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(co => co.Quantity).HasColumnType("Decimal").HasPrecision(15, 2).IsRequired(true);
            builder.Property(co => co.ReservedQuantity).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.HasIndex(p => new { p.ProductID, p.UnitChartID }).HasDatabaseName("IX_Inventory").IsUnique(true);
        }
    }
}
