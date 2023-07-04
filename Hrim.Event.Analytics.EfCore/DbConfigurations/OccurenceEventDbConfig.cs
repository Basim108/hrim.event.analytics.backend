using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class OccurenceEventDbConfig: IEntityTypeConfiguration<DbOccurrenceEvent>
{
    public void Configure(EntityTypeBuilder<DbOccurrenceEvent> builder) {
        builder.ToTable(name: "occurrence_events",
                        t =>
                            t.HasComment(comment: "When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day."));

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
               .HasComment(comment: "Date when an event occurred")
               .IsRequired();

        builder.Property(p => p.OccurredAt)
               .HasColumnName(nameof(DbOccurrenceEvent.OccurredAt).ToSnakeCase())
               .HasComment(comment: "Time with end-user timezone when an event occurred")
               .HasColumnType(typeName: "timetz")
               .IsRequired();
        
        builder.HasMany(x => x.AnalysisResults)
               .WithOne()
               .HasForeignKey(x => x.EntityId)
               .IsRequired();
    }
}