using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class EventTypeDbConfig: IEntityTypeConfiguration<DbEventType>
{
    public void Configure(EntityTypeBuilder<DbEventType> builder) {
        builder.ToTable(name: "event_types",
                        t =>
                            t.HasComment(comment: "User defined event types.\nhttps://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types"));

        builder.AddEntityProperties<DbEventType, long>();

        builder.HasIndex(x => new {
                    x.CreatedById,
                    x.Name
                })
               .IsUnique();

        builder.Property(p => p.ParentId)
               .HasColumnName(nameof(DbEventType.ParentId).ToSnakeCase())
               .HasComment(comment: "Reference to a more general event type, which this type is specified in some context\nFor example, if current event type is Hatha Yoga, its parent type might be just general Yoga.");
        
        builder.Property(p => p.Name)
               .HasColumnName(nameof(EventType.Name).ToSnakeCase())
               .HasComment(comment: "Event type name, e.g. 'nice mood', 'headache', etc")
               .IsRequired();

        builder.Property(p => p.Description)
               .HasColumnName(nameof(EventType.Description).ToSnakeCase())
               .HasComment(comment: "Description given by user, when user_event_type based on this one will be created.");

        builder.Property(p => p.Color)
               .HasColumnName(nameof(EventType.Color).ToSnakeCase())
               .HasComment(comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'");

        builder.Property(p => p.IsPublic)
               .HasColumnName(nameof(EventType.IsPublic).ToSnakeCase())
               .HasComment(comment: " An owner who created this event_type could share it with other end-users");

        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(EventType.CreatedBy).ToSnakeCase())
               .HasComment(comment: "A user who created an instance of this event type")
               .IsRequired();

        builder.Property(p => p.TreeNodePath)
               .HasColumnName(nameof(DbEventType.TreeNodePath).ToSnakeCase());
        
        builder.HasOne(x => x.CreatedBy);
        
        builder.HasOne(x => x.Parent)
               .WithMany(x => x.Children)
               .HasForeignKey(x => x.ParentId);
        
        builder.HasMany(x => x.AnalysisResults)
               .WithOne()
               .HasForeignKey(x => x.EntityId)
               .IsRequired();
    }
}