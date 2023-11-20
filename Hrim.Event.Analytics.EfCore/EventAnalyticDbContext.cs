using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore.DbConfigurations;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace Hrim.Event.Analytics.EfCore;

public class EventAnalyticDbContext: DbContext
{
    public EventAnalyticDbContext(DbContextOptions<EventAnalyticDbContext> options)
        : base(options: options) { }

    public DbSet<DbEventType>         EventTypes           { get; set; }
    public DbSet<DbDurationEvent>     DurationEvents       { get; set; }
    public DbSet<DbOccurrenceEvent>   OccurrenceEvents     { get; set; }
    public DbSet<HrimUser>            HrimUsers            { get; set; }
    public DbSet<ExternalUserProfile> ExternalUserProfiles { get; set; }
    public DbSet<HrimTag>             HrimTags             { get; set; }
    public DbSet<HrimFeature>         HrimFeatures         { get; set; }

    // TODO: try use contravariant
    public DbSet<DbAnalysisConfigByEventType> AnalysisByEventType     { get; set; }
    public DbSet<StatisticsForEvent>          StatisticsForEvents     { get; set; }
    public DbSet<StatisticsForEventType>      StatisticsForEventTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema(schema: "v2_hrim_analytics");
        modelBuilder.HasPostgresExtension(name: "uuid-ossp"); // enables guid generation functions e.g. uuid_generate_v4

        modelBuilder.ApplyConfiguration(new HrimUserDbConfig());
        modelBuilder.ApplyConfiguration(new ExternalUserProfileDbConfig());
        modelBuilder.ApplyConfiguration(new EventTypeDbConfig());
        modelBuilder.ApplyConfiguration(new DurationEventDbConfig());
        modelBuilder.ApplyConfiguration(new OccurenceEventDbConfig());
        modelBuilder.ApplyConfiguration(new HrimTagDbConfig());
        modelBuilder.ApplyConfiguration(new HrimFeatureDbConfig());
        modelBuilder.ApplyConfiguration(new AnalysisByEventTypeDbConfig());
        modelBuilder.ApplyConfiguration(new StatisticsForEventDbConfig());
        modelBuilder.ApplyConfiguration(new StatisticsForEventTypeDbConfig());
    }
}