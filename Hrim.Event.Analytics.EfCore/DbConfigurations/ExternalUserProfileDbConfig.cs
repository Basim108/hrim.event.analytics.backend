using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class ExternalUserProfileDbConfig: IEntityTypeConfiguration<ExternalUserProfile> {
    public void Configure(EntityTypeBuilder<ExternalUserProfile> builder) {
        builder.ToTable("external_user_profiles", 
                        t => t.HasComment("user profiles from a specific idp such as Google, Facebook, etc"));

        builder.AddEntityProperties();

        builder.Property(p => p.HrimUserId)
               .HasColumnName("user_id")
               .HasComment("A user id in current system to which this profile is linked to")
               .IsRequired();
        
        builder.Property(p => p.ExternalUserId)
               .HasColumnName(nameof(ExternalUserProfile.ExternalUserId).ToSnakeCase())
               .HasComment("A user id in external identity provider")
               .IsRequired();

        builder.Property(p => p.Email)
               .HasColumnName(nameof(ExternalUserProfile.Email).ToSnakeCase())
               .IsRequired();
        
        builder.Property(p => p.Idp)
               .HasColumnName(nameof(ExternalUserProfile.Idp).ToSnakeCase())
               .HasComment("Identity provider that provided this profile")
               .HasConversion(EnumToSnakeCaseConverter<ExternalIdp>.Get())
               .IsRequired();
        
        builder.Property(p => p.LastLogin)
               .HasColumnName(nameof(ExternalUserProfile.LastLogin).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment("If null then profile was linked but never used as a login")
               .IsRequired();
        
        builder.Property(p => p.FullName)
               .HasColumnName(nameof(ExternalUserProfile.FullName).ToSnakeCase());

        builder.Property(p => p.FirstName)
               .HasColumnName(nameof(ExternalUserProfile.FirstName).ToSnakeCase());
           
        builder.Property(p => p.LastName)
               .HasColumnName(nameof(ExternalUserProfile.LastName).ToSnakeCase());
    }
}