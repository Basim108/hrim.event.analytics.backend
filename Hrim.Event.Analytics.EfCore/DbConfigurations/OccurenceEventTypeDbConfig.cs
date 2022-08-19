using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class OccurenceEventTypeDbConfig: IEntityTypeConfiguration<DbOccurrenceEventType> {
    public void Configure(EntityTypeBuilder<DbOccurrenceEventType> builder) {
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

        builder.Property(p => p.OccurredOn)
               .HasColumnName(nameof(DbOccurrenceEventType.OccurredOn).ToSnakeCase())
               .HasComment("Date when an event occurred")
               .IsRequired();
        builder.Property(p => p.OccurredAt)
               .HasColumnName(nameof(DbOccurrenceEventType.OccurredAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event occurred")
               .HasColumnType("timetz")
               .IsRequired();
    }
}