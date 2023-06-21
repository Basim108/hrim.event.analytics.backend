using Hrim.Event.Analytics.Abstractions.Entities;
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
               .HasComment(@"Could be one of:
Analysis")
               .IsRequired();
        
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
    }
}