using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class EShopUserConfiguration : IEntityTypeConfiguration<EShopUser>
    {
        public EShopUserConfiguration()
        {           

        }

        public void Configure(EntityTypeBuilder<EShopUser> builder)
        { 
            builder.Property(k=>k.UserName).IsRequired(true).HasMaxLength(20);
            builder.Property(k => k.FirstName).IsRequired(true).HasMaxLength(200);
            builder.Property(k => k.LastName).IsRequired(false).HasMaxLength(200);
            builder.Property(k => k.Address).IsRequired(false).HasMaxLength(1000);
            builder.Property(k => k.Email).IsRequired(true).HasMaxLength(250);
            builder.Property(k => k.Password).IsRequired(true).HasMaxLength(100);
            builder.Property(k => k.IsActive).IsRequired(true);
            builder.Property(k => k.ActivationCode).IsRequired(true); 
        }
    }
}
