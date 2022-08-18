using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class DurationEventTypeDbConfig: IEntityTypeConfiguration<DurationEventType> {
    public void Configure(EntityTypeBuilder<DurationEventType> builder) {
        builder.ToTable("duration_event_types")
               .HasComment("When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day.");

        builder.HasIndex(x => new {
                             x.CreatedById,
                             x.StartedAt
                         })
               .IncludeProperties(x => new {
                                      x.FinishedAt,
                                      x.Color,
                                      x.Name
                                  });

        builder.AddEntityProperties();
        builder.AddSystemEventTypeProperties();

        builder.Property(p => p.StartedAt)
               .HasColumnName(nameof(DurationEventType.StartedAt).ToSnakeCase())
               .HasComment("Date and time with end-user timezone when an event starts")
               .HasColumnType("timestamptz")
               .IsRequired();

        builder.Property(p => p.FinishedAt)
               .HasColumnName(nameof(DurationEventType.FinishedAt).ToSnakeCase())
               .HasComment("Date and time with end-user timezone when an event finishes")
               .HasColumnType("timestamptz")
               .IsRequired();
    }
}