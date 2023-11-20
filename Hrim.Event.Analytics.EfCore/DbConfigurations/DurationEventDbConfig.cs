using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrim.Event.Analytics.EfCore.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class DurationEventDbConfig: IEntityTypeConfiguration<DbDurationEvent>
{
    public void Configure(EntityTypeBuilder<DbDurationEvent> builder) {
        builder.ToTable(name: "duration_events",
                        t =>
                            t.HasComment(comment:
                                         "When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day and can cross each other.")
                       );

        builder.HasIndex(x => new {
                    x.CreatedById,
                    x.StartedOn
                })
               .IncludeProperties(x => new {
                    x.EventTypeId,
                    x.StartedAt,
                    x.FinishedOn,
                    x.FinishedAt,
                    x.IsDeleted
                });

        builder.AddEntityProperties<DbDurationEvent, long>();
        
        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(DbDurationEvent.CreatedBy).ToSnakeCase())
               .HasComment(comment: "A user who created an instance of this event type")
               .IsRequired();
        builder.HasOne(x => x.CreatedBy);

        builder.Property(p => p.EventTypeId)
               .HasColumnName(nameof(DbDurationEvent.EventTypeId).ToSnakeCase())
               .HasComment(comment: "Event type on which current event is based.")
               .IsRequired();
        builder.HasOne(x => x.EventType);
        
        builder.Property(p => p.Props)
               .HasColumnName(nameof(DbDurationEvent.Props).ToSnakeCase())
               .HasComment("Some additional values associated with this event")
               .HasConversion(JsonDictionaryConverter.GetNullable())
               .HasColumnType("jsonb");

        builder.Property(p => p.StartedOn)
               .HasColumnName(nameof(DbDurationEvent.StartedOn).ToSnakeCase())
               .HasComment(comment: "Date when an event started")
               .IsRequired();
        builder.Property(p => p.StartedAt)
               .HasColumnName(nameof(DbDurationEvent.StartedAt).ToSnakeCase())
               .HasComment(comment: "Time with end-user timezone when an event started")
               .HasColumnType(typeName: "timetz")
               .IsRequired();
        builder.Property(p => p.FinishedOn)
               .HasColumnName(nameof(DbDurationEvent.FinishedOn).ToSnakeCase())
               .HasComment(comment: "Date when an event finished");
        builder.Property(p => p.FinishedAt)
               .HasColumnName(nameof(DbDurationEvent.FinishedAt).ToSnakeCase())
               .HasComment(comment: "Time with end-user timezone when an event finished")
               .HasColumnType(typeName: "timetz");
        
        builder.HasMany(x => x.AnalysisResults)
               .WithOne()
               .HasForeignKey(x => x.EntityId)
               .IsRequired();
    }
}