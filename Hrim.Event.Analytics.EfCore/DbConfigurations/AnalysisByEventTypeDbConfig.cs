using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.ValueConverters;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class AnalysisByEventTypeDbConfig: IEntityTypeConfiguration<AnalysisByEventType>
{
    public void Configure(EntityTypeBuilder<AnalysisByEventType> builder) {
        builder.ToTable(name: nameof(AnalysisByEventType).ToSnakeCase(), 
                        schema: "analysis",  
                        t => {
                            t.HasComment(comment: "Analysis that is made around events of a particular event-type");
                            
                            var concurrentColumn    = nameof(HrimEntity.ConcurrentToken).ToSnakeCase();
                            var checkConstraintName = $"CK_{nameof(AnalysisByEventType).ToSnakeCase()}s_{concurrentColumn}";
                            t.HasCheckConstraint(name: checkConstraintName, $"{concurrentColumn} > 0");
                        });

        builder.HasKey(x => new {
            x.EventTypeId,
            x.AnalysisCode
        });
        
        builder.Property(p => p.EventTypeId)
               .HasColumnName(nameof(AnalysisByEventType.EventTypeId).ToSnakeCase())
               .HasComment("Events of this event type will be analysed")
               .IsRequired();
        
        builder.Property(p => p.AnalysisCode)
               .HasColumnName(nameof(AnalysisByEventType.AnalysisCode).ToSnakeCase())
               .IsRequired();
        
        builder.Property(p => p.IsOn)
               .HasColumnName(nameof(AnalysisByEventType.IsOn).ToSnakeCase())
               .HasComment("Enable/disable analysis for a particular event-type")
               .IsRequired();

        builder.Property(p => p.Settings)
               .HasColumnName(nameof(AnalysisByEventType.Settings).ToSnakeCase())
               .HasConversion(JsonDictionaryConverter.GetNullable())
               .HasColumnType("jsonb");

        builder.Property(p => p.CreatedAt)
               .HasColumnName(nameof(AnalysisByEventType.CreatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance creation")
               .HasColumnType(typeName: "timestamptz")
               .IsRequired();

        builder.Property(p => p.UpdatedAt)
               .HasColumnName(nameof(AnalysisByEventType.UpdatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance last update ")
               .HasColumnType(typeName: "timestamptz");
        
        builder.Property(p => p.ConcurrentToken)
               .HasColumnName(nameof(HrimEntity.ConcurrentToken).ToSnakeCase())
               .HasComment(comment: "Update is possible only when this token equals to the token in the storage")
               .IsConcurrencyToken()
               .IsRequired();

        builder.HasOne<UserEventType>(x => x.EventType)
               .WithMany(x => x.AnalysisSettings);
    }
}