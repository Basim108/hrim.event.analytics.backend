using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class DurationEventTypeDbConfig: IEntityTypeConfiguration<DbDurationEventType> {
    public void Configure(EntityTypeBuilder<DbDurationEventType> builder) {
        builder.ToTable("duration_event_types")
               .HasComment("When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day.");

        builder.HasIndex(x => new {
                             x.CreatedById,
                             x.StartedOn
                         })
               .IncludeProperties(x => new {
                                      x.StartedAt,
                                      x.FinishedOn,
                                      x.FinishedAt,
                                      x.Color,
                                      x.Name
                                  });

        builder.AddEntityProperties();
        builder.AddSystemEventTypeProperties();

        builder.Property(p => p.StartedOn)
               .HasColumnName(nameof(DbDurationEventType.StartedOn).ToSnakeCase())
               .HasComment("Date when an event started")
               .IsRequired();
        builder.Property(p => p.StartedAt)
               .HasColumnName(nameof(DbDurationEventType.StartedAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event started")
               .HasColumnType("timetz")
               .IsRequired();
        builder.Property(p => p.FinishedOn)
               .HasColumnName(nameof(DbDurationEventType.FinishedOn).ToSnakeCase())
               .HasComment("Date when an event finished");
        builder.Property(p => p.FinishedAt)
               .HasColumnName(nameof(DbDurationEventType.FinishedAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event finished")
               .HasColumnType("timetz");
    }
}