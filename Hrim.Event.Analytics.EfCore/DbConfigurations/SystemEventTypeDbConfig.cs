using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class SystemEventTypeDbConfig: IEntityTypeConfiguration<SystemEventType> {
    public void Configure(EntityTypeBuilder<SystemEventType> builder) {
        builder.ToTable("event_types")
               .HasComment("User defined event types.\nhttps://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types");

        builder.AddEntityProperties();

        builder.HasIndex(x => new { x.CreatedById, x.Name })
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
               .HasComment(" An owner who created this event_type could share it with other end-users");

        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(SystemEventType.CreatedBy).ToSnakeCase())
               .HasComment("A user who created an instance of this event type")
               .IsRequired();
        builder.HasOne(x => x.CreatedBy);
        
        builder.Property(p => p.EventType)
               .HasColumnName(nameof(SystemEventType.EventType).ToSnakeCase())
               .HasComment("Specifies a type of events that will be registered: duration, occurrence, etc")
               .HasConversion(new EnumToStringConverter<EventType>())
               .IsRequired();
    }
}