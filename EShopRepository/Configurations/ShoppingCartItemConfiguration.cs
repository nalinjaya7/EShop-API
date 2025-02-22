using EShopModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopRepository.Configurations
{
    public class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemConfiguration()
        {

        }

        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.Property(co => co.ShoppingCartID).IsRequired(true);
            builder.Property(co => co.UnitChartID).IsRequired(true);
            builder.Property(co => co.ProductID).IsRequired(true);
            builder.Property(co => co.UnitPrice).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(co => co.LineDiscount).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(pr => pr.Quantity).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
        }
    }
}
