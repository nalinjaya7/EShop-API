using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EShopModels;

namespace EShopRepository.Configurations
{
    public class LoginConfiguration : IEntityTypeConfiguration<Login>
    {
        public LoginConfiguration()
        {

        }

        public void Configure(EntityTypeBuilder<Login> builder)
        {
            builder.Property(co => co.UserName).IsRequired(true).HasMaxLength(200).IsUnicode(false);
            builder.HasIndex(u => new { u.UserName }).HasDatabaseName("IX_LoginUserNameUnique").IsUnique(true);
            builder.Property(p => p.Password).HasMaxLength(200).HasColumnType("NVARCHAR").HasMaxLength(200).IsRequired(true);
            builder.Property(co => co.IsRemember).IsRequired(true);
        }
    }
}
