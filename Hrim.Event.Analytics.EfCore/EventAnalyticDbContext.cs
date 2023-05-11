using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbConfigurations;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace Hrim.Event.Analytics.EfCore;

public class EventAnalyticDbContext: DbContext
{
    public EventAnalyticDbContext(DbContextOptions<EventAnalyticDbContext> options)
        : base(options: options) { }

    public DbSet<UserEventType>       UserEventTypes       { get; set; }
    public DbSet<DbDurationEvent>     DurationEvents       { get; set; }
    public DbSet<DbOccurrenceEvent>   OccurrenceEvents     { get; set; }
    public DbSet<HrimUser>            HrimUsers            { get; set; }
    public DbSet<ExternalUserProfile> ExternalUserProfiles { get; set; }
    public DbSet<HrimTag>             HrimTags             { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema(schema: "hrim_analytics");
        modelBuilder.HasPostgresExtension(name: "uuid-ossp"); // enables guid generation functions e.g. uuid_generate_v4

        modelBuilder.ApplyConfiguration(new HrimUserDbConfig());
        modelBuilder.ApplyConfiguration(new ExternalUserProfileDbConfig());
        modelBuilder.ApplyConfiguration(new UserEventTypeDbConfig());
        modelBuilder.ApplyConfiguration(new DurationEventDbConfig());
        modelBuilder.ApplyConfiguration(new OccurenceEventDbConfig());
        modelBuilder.ApplyConfiguration(new HrimTagDbConfig());
    }
}