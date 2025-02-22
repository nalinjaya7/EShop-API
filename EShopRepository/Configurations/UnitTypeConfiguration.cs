using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class UnitTypeConfiguration : IEntityTypeConfiguration<UnitType>
    {
        public UnitTypeConfiguration()
        {            

        }

        public void Configure(EntityTypeBuilder<UnitType> builder)
        {
            builder.Property(co => co.IsBaseUnit).IsRequired(true);
            builder.Property(co => co.Code).HasMaxLength(15).IsUnicode(false);
            builder.Property(co => co.Name).HasMaxLength(20).IsUnicode(true);
            builder.HasIndex(u => new { u.Name }).HasDatabaseName("IX_UnitTypeName").IsUnique(true);
        }
    }
}
