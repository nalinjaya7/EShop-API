using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class ProductSubCategoryConfiguration : IEntityTypeConfiguration<ProductSubCategory>
    {
        public ProductSubCategoryConfiguration()
        {  

        }

        public void Configure(EntityTypeBuilder<ProductSubCategory> builder)
        {
            builder.Property(co => co.Name).IsRequired(true).HasMaxLength(100).IsUnicode(false);
            builder.Property(g => g.ProductCategoryID).IsRequired(true);
        }
    }
}
