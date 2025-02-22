using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryConfiguration()
        {
           
        }

        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.Property(co => co.Name).IsRequired(true).HasMaxLength(100).IsUnicode(false);
            builder.HasIndex(u => new { u.Name }).HasDatabaseName("IX_ProductCategoryName").IsUnique(true);
        }
    }
}
