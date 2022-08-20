using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class OccurenceEventDbConfig: IEntityTypeConfiguration<DbOccurrenceEvent> {
    public void Configure(EntityTypeBuilder<DbOccurrenceEvent> builder) {
        builder.ToTable("occurrence_events")
               .HasComment("When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

        builder.HasIndex(x => new {
                             x.CreatedById,
                             x.OccurredAt
                         })
               .IncludeProperties(x => x.IsPublic);

        builder.AddEntityProperties();

        builder.Property(p => p.OccurredOn)
               .HasColumnName(nameof(DbOccurrenceEvent.OccurredOn).ToSnakeCase())
               .HasComment("Date when an event occurred")
               .IsRequired();
        
        builder.Property(p => p.OccurredAt)
               .HasColumnName(nameof(DbOccurrenceEvent.OccurredAt).ToSnakeCase())
               .HasComment("Time with end-user timezone when an event occurred")
               .HasColumnType("timetz")
               .IsRequired();
        
        builder.Property(p => p.IsPublic)
               .HasColumnName(nameof(DbOccurrenceEvent.IsPublic).ToSnakeCase())
               .HasComment("An owner who created this event_type could share it with other end-users.\nWill override IsPublic value of an event_type instance");
    }
}