using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {           

        }

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(co => co.ProductSubCategoryID).IsRequired(false);
            builder.Property(co => co.ProductCategoryID).IsRequired(true); 
            builder.Property(co => co.Name).IsUnicode(false).IsRequired(true).HasMaxLength(200); 
            builder.Property(co => co.BarCode).IsUnicode(false).HasMaxLength(20).IsRequired(true);
            builder.Property(co => co.ItemCode).IsUnicode(false).HasMaxLength(20).IsRequired(true);
            builder.Property(pr => pr.TaxRate).HasColumnType("Decimal").HasPrecision(15, 2).IsRequired(false);

            builder.Property(co => co.ReOrderLevel).IsRequired(true);
            builder.Property(co => co.TaxGroupID).IsRequired(false);
            builder.Property(co => co.TaxInclude).IsRequired(true);
            builder.Property(co => co.TaxRate).IsRequired(false);
            builder.Property(co => co.ProductImage).HasMaxLength(4000).IsRequired(false);

            builder.HasIndex(u => new { u.BarCode }).HasDatabaseName("IX_ProductBarCodeUnique").IsUnique(true);
            builder.HasIndex(u => new { u.ItemCode }).HasDatabaseName("IX_ProductItemCodeUnique").IsUnique(true);
        }
    }
}
