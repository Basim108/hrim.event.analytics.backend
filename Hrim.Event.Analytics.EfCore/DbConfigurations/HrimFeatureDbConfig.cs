using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class HrimFeatureDbConfig: IEntityTypeConfiguration<HrimFeature>
{
    public void Configure(EntityTypeBuilder<HrimFeature> builder) {
        builder.ToTable(name: "hrim_features",
                        t => t.HasComment(comment: @"Features that might be on/off;
for example, analysis based on event-types, tags, events, etc"));

        builder.AddEntityProperties();

        builder.Property(p => p.FeatureType)
               .HasColumnName(nameof(HrimFeature.FeatureType).ToSnakeCase())
               .HasConversion<string>()
               .HasComment(@"Could be one of:
Analysis")
               .IsRequired();

        builder.Property(p => p.Description)
               .HasColumnName(nameof(HrimFeature.Description).ToSnakeCase());

        builder.Property(p => p.Code)
               .HasColumnName(nameof(HrimFeature.Code).ToSnakeCase())
               .HasComment("Feature code")
               .IsRequired();

        builder.Property(p => p.VariableName)
               .HasColumnName(nameof(HrimFeature.VariableName).ToSnakeCase())
               .HasComment("Environment variable name that controls is this feature set on/off")
               .IsRequired();

        builder.Property(p => p.IsOn)
               .HasColumnName(nameof(HrimFeature.IsOn).ToSnakeCase())
               .HasComment(@"When a feature is off then its hangfire jobs, in case existed, should not be proceeded or scheduled.
and in case feature represents an analysis (e.g. count, gap) this analysis should not appear in the list of available analysis.")
               .IsRequired();

        builder.HasData(new HrimFeature {
            Id              = Guid.Parse("2F1E83AA-A0F2-492F-AF76-C6DE43AD277B"),
            Code            = FeatureCodes.GAP_ANALYSIS,
            VariableName    = "FEAT_GAP_ANALYSIS",
            FeatureType     = FeatureType.Analysis,
            Description     = "Calculates gaps between events of a specific event type",
            IsOn            = true,
            CreatedAt       = DateTime.Parse("2023-06-21 21:02:52.864000 +00:00"),
            UpdatedAt       = DateTime.Parse("2023-06-21 21:02:52.864000 +00:00"),
            ConcurrentToken = 1
        });
    }
}