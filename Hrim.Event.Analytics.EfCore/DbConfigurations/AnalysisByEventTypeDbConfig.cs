using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;
using Hrim.Event.Analytics.EfCore.ValueConverters;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class AnalysisByEventTypeDbConfig: IEntityTypeConfiguration<DbAnalysisConfigByEventType>
{
    public void Configure(EntityTypeBuilder<DbAnalysisConfigByEventType> builder) {
        builder.ToTable(name: nameof(AnalysisConfigByEventType).ToSnakeCase(), 
                        schema: "v2_analysis",  
                        t => {
                            t.HasComment(comment: "Configuration of an analysis that will be made around events of a particular event-type");
                            
                            var concurrentColumn    = nameof(HrimEntity<int>.ConcurrentToken).ToSnakeCase();
                            var checkConstraintName = $"CK_{nameof(AnalysisConfigByEventType).ToSnakeCase()}s_{concurrentColumn}";
                            t.HasCheckConstraint(name: checkConstraintName, $"{concurrentColumn} > 0");
                        });

        builder.HasKey(x => new {
            x.EventTypeId,
            x.AnalysisCode
        });
        
        builder.Property(p => p.EventTypeId)
               .HasColumnName(nameof(AnalysisConfigByEventType.EventTypeId).ToSnakeCase())
               .HasComment("Events of this event type will be analysed")
               .IsRequired();
        
        builder.Property(p => p.AnalysisCode)
               .HasColumnName(nameof(AnalysisConfigByEventType.AnalysisCode).ToSnakeCase())
               .IsRequired();
        
        builder.Property(p => p.IsOn)
               .HasColumnName(nameof(AnalysisConfigByEventType.IsOn).ToSnakeCase())
               .HasComment("Enable/disable analysis for a particular event-type")
               .IsRequired();

        builder.Property(p => p.Settings)
               .HasColumnName(nameof(AnalysisConfigByEventType.Settings).ToSnakeCase())
               .HasConversion(JsonDictionaryConverter.GetNullable())
               .HasColumnType("jsonb");

        builder.Property(p => p.CreatedAt)
               .HasColumnName(nameof(AnalysisConfigByEventType.CreatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance creation")
               .HasColumnType(typeName: "timestamptz")
               .IsRequired();

        builder.Property(p => p.UpdatedAt)
               .HasColumnName(nameof(AnalysisConfigByEventType.UpdatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance last update ")
               .HasColumnType(typeName: "timestamptz");
        
        builder.Property(p => p.ConcurrentToken)
               .HasColumnName(nameof(HrimEntity<int>.ConcurrentToken).ToSnakeCase())
               .HasComment(comment: "Update is possible only when this token equals to the token in the storage")
               .IsConcurrencyToken()
               .IsRequired();

        builder.HasOne<DbEventType>(x => x.EventType)
               .WithMany(x => x.AnalysisSettings);
    }
}