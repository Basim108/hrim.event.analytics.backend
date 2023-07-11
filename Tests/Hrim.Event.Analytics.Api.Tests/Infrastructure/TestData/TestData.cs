using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.EfCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class TestData
{
    public EventAnalyticDbContext DbContext { get; private set; }

    public TestData(EventAnalyticDbContext context) {
        DbContext           = context;
        Events              = new EventsData(context);
        Users               = new UsersData(context);
        Features            = new FeaturesData(context);
        AnalysisByEventType = new AnalysisByEventTypeData(context);
        AnalysisResults     = new EventTypeAnalysisResultsData(context);
    }

    public EventsData                   Events              { get; }
    public UsersData                    Users               { get; }
    public FeaturesData                 Features            { get; }
    public AnalysisByEventTypeData      AnalysisByEventType { get; }
    public EventTypeAnalysisResultsData AnalysisResults     { get; }
}