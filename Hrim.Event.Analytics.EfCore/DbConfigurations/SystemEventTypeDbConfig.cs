using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public static class SystemEventTypeDbConfig {
    public static void AddSystemEventTypeProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : SystemEventType {

        builder.HasIndex(x => new { x.CreatedById, x.Name})
               .IsUnique();
        
        builder.Property(p => p.Name)
               .HasColumnName(nameof(SystemEventType.Name).ToSnakeCase())
               .HasComment("Event type name, e.g. 'nice mood', 'headache', etc")
               .IsRequired();
        
        builder.Property(p => p.Description)
               .HasColumnName(nameof(SystemEventType.Description).ToSnakeCase())
               .HasComment("Description given by user, when user_event_type based on this one will be created.");
        
        builder.Property(p => p.Color)
               .HasColumnName(nameof(SystemEventType.Color).ToSnakeCase())
               .HasComment("A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'");

        builder.Property(p => p.IsPublic)
               .HasColumnName(nameof(SystemEventType.IsPublic).ToSnakeCase())
               .HasComment("A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'");

        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(SystemEventType.CreatedBy).ToSnakeCase())
               .HasComment("A user who created an instance of this event type")
               .IsRequired();
        builder.HasOne(x => x.CreatedBy);
    }
}