using EShopModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopRepository.Configurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public ShoppingCartConfiguration()
        {

        }

        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.Property(co => co.UserID).IsRequired(true);
            builder.Property(co => co.ShoppingCartStatus).IsRequired(true);
            builder.Property(co => co.GrossAmount).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(co => co.DiscountAmount).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2);
            builder.Property(co => co.TaxAmount).IsRequired(true).HasColumnType("Decimal").HasPrecision(15, 2); 
        }
    }
}
