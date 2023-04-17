using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class OccurenceEventDbConfig: IEntityTypeConfiguration<DbOccurrenceEvent> {
    public void Configure(EntityTypeBuilder<DbOccurrenceEvent> builder) {
        builder.ToTable("occurrence_events",
                        t => 
                            t.HasComment("When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day."));

        builder.HasIndex(x => new {
                    x.CreatedById,
                    x.OccurredAt
                })
               .IncludeProperties(x => new {
                    x.EventTypeId,
                    x.IsDeleted
                });

        builder.AddEntityProperties();
        builder.AddEventBaseProperties();
        
        builder.Property(p => p.OccurredOn)
               .HasColumnName(nameof(DbOccurrenceEvent.OccurredOn).ToSnakeCase())
               .HasComment("Date when an event occurred")
               .IsRequired();
        
        builder.Property(p => p.OccurredAt)
               .HasColumnName(nameof(DbOccurrenceEvent.OccurredAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event occurred")
               .HasColumnType("timetz")
               .IsRequired();
    }
}