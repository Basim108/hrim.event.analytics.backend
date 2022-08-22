using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class DurationEventDbConfig: IEntityTypeConfiguration<DbDurationEvent> {
    public void Configure(EntityTypeBuilder<DbDurationEvent> builder) {
        builder.ToTable("duration_events")
               .HasComment("When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day and can cross each other.");

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

        builder.AddEntityProperties();
        builder.AddEventBaseProperties();

        builder.Property(p => p.StartedOn)
               .HasColumnName(nameof(DbDurationEvent.StartedOn).ToSnakeCase())
               .HasComment("Date when an event started")
               .IsRequired();
        builder.Property(p => p.StartedAt)
               .HasColumnName(nameof(DbDurationEvent.StartedAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event started")
               .HasColumnType("timetz")
               .IsRequired();
        builder.Property(p => p.FinishedOn)
               .HasColumnName(nameof(DbDurationEvent.FinishedOn).ToSnakeCase())
               .HasComment("Date when an event finished");
        builder.Property(p => p.FinishedAt)
               .HasColumnName(nameof(DbDurationEvent.FinishedAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event finished")
               .HasColumnType("timetz");
    }
}