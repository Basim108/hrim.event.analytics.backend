using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class ExternalUserProfileDbConfig: IEntityTypeConfiguration<ExternalUserProfile>
{
    public void Configure(EntityTypeBuilder<ExternalUserProfile> builder) {
        builder.ToTable(name: "external_user_profiles",
                        t => t.HasComment(comment: "user profiles from a specific idp such as Google, Facebook, etc"));

        builder.AddEntityProperties<ExternalUserProfile, long>();

        builder.Property(p => p.HrimUserId)
               .HasColumnName(name: "user_id")
               .HasComment(comment: "A user id in current system to which this profile is linked to")
               .IsRequired();

        builder.Property(p => p.ExternalUserId)
               .HasColumnName(nameof(ExternalUserProfile.ExternalUserId).ToSnakeCase())
               .HasComment(comment: "A user id in external identity provider")
               .IsRequired();

        builder.Property(p => p.Email)
               .HasColumnName(nameof(ExternalUserProfile.Email).ToSnakeCase());

        builder.Property(p => p.Idp)
               .HasColumnName(nameof(ExternalUserProfile.Idp).ToSnakeCase())
               .HasComment(comment: "Identity provider that provided this profile")
               .HasConversion(EnumToSnakeCaseConverter<ExternalIdp>.Get())
               .IsRequired();

        builder.Property(p => p.LastLogin)
               .HasColumnName(nameof(ExternalUserProfile.LastLogin).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "If null then profile was linked but never used as a login")
               .IsRequired();

        builder.Property(p => p.FullName)
               .HasColumnName(nameof(ExternalUserProfile.FullName).ToSnakeCase());

        builder.Property(p => p.FirstName)
               .HasColumnName(nameof(ExternalUserProfile.FirstName).ToSnakeCase());

        builder.Property(p => p.LastName)
               .HasColumnName(nameof(ExternalUserProfile.LastName).ToSnakeCase());
    }
}