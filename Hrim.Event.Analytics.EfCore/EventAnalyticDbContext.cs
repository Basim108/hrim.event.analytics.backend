using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbConfigurations;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace Hrim.Event.Analytics.EfCore;

public class EventAnalyticDbContext: DbContext {
    public EventAnalyticDbContext(DbContextOptions<EventAnalyticDbContext> options)
        : base(options) { }

    public DbSet<DurationEventType>   DurationEventTypes   { get; set; }
    public DbSet<OccurrenceEventType> OccurrenceEventTypes { get; set; }
    public DbSet<HrimTag>             HrimTags             { get; set; }
    public DbSet<HrimUser>            HrimUsers            { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema("hrim_analytics");
        modelBuilder.HasPostgresExtension("uuid-ossp"); // enables guid generation functions e.g. uuid_generate_v4

        modelBuilder.ApplyConfiguration(new HrimUserDbConfig());
        modelBuilder.ApplyConfiguration(new DurationEventTypeDbConfig());
        modelBuilder.ApplyConfiguration(new OccurenceEventTypeDbConfig());
        modelBuilder.ApplyConfiguration(new HrimTagDbConfig());
    }
}