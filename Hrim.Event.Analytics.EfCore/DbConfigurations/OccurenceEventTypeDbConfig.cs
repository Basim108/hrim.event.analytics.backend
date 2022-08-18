using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class OccurenceEventTypeDbConfig: IEntityTypeConfiguration<OccurrenceEventType> {
    public void Configure(EntityTypeBuilder<OccurrenceEventType> builder) {
        builder.ToTable("occurrence_event_types")
               .HasComment("When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

        builder.HasIndex(x => new {
                             x.CreatedById,
                             x.OccurredAt
                         })
               .IncludeProperties(x => new {
                                      x.Color,
                                      x.Name
                                  });

        builder.AddEntityProperties();
        builder.AddSystemEventTypeProperties();

        builder.Property(p => p.OccurredAt)
               .HasColumnName(nameof(OccurrenceEventType.OccurredAt).ToSnakeCase())
               .HasComment("Date and time with end-user timezone when an event occurred")
               .HasColumnType("timestamptz")
               .IsRequired();
    }
}