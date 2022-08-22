using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public static class EventBaseDbConfig {
    public static void AddEventBaseProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEvent {

        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(BaseEvent.CreatedBy).ToSnakeCase())
               .HasComment("A user who created an instance of this event type")
               .IsRequired();
        builder.HasOne(x => x.CreatedBy);
        
        builder.Property(p => p.EventTypeId)
               .HasColumnName(nameof(BaseEvent.EventTypeId).ToSnakeCase())
               .HasComment("Event type on which current event is based.")
               .IsRequired();
        builder.HasOne(x => x.EventType);
    }
}