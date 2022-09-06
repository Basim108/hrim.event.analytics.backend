using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.EfCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestData {
    public readonly EventAnalyticDbContext DbContext;
    public TestData(EventAnalyticDbContext context) {
        DbContext = context;
        Events = new EventsData(context);
        Users  = new UsersData(context);
    }
    
    public EventsData Events {get;}
    public UsersData Users {get;}
}