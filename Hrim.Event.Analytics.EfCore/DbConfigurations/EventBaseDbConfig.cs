using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public static class EventBaseDbConfig {
    public static void AddEventBaseProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : EventBase {

        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(EventBase.CreatedBy).ToSnakeCase())
               .HasComment("A user who created an instance of this event type")
               .IsRequired();
        builder.HasOne(x => x.CreatedBy);
        
        builder.Property(p => p.IsPublic)
               .HasColumnName(nameof(EventBase.IsPublic).ToSnakeCase())
               .HasComment("An owner who created this event_type could share it with other end-users.\nWill override IsPublic value of an event_type instance");
    }
}