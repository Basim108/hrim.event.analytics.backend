using Hrim.Event.Analytics.Abstractions.Entities;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations; 

public class HrimUserDbConfig: IEntityTypeConfiguration<HrimUser> {
    public void Configure(EntityTypeBuilder<HrimUser> builder) {
        builder.ToTable("hrim_users")
               .HasComment("An authorized user");
        
        builder.AddEntityProperties();
        
        builder.Property(p => p.Email)
               .HasColumnName(nameof(HrimUser.Email).ToSnakeCase())
               .IsRequired();
    }
}