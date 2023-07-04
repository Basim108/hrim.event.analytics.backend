using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.GapAnalysis;

public class GapAnalysisRecurringJobTests
{
    private readonly GapAnalysisRecurringJobHandler _handler;
    private readonly GapAnalysisRecurringJob        _job = new (Guid.NewGuid());
    private readonly IDictionary<string, string>    _settings;
    private readonly IMediator                      _mediator = Substitute.For<IMediator>();
    private readonly EventAnalyticDbContext         _context  = DbUtils.GetDbContext();
    private readonly TestData                       _testData;

    public GapAnalysisRecurringJobTests() {
        _testData = new TestData(_context);
        _settings = new Dictionary<string, string>() {
            { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1:00:00:00"}
        };
        _handler = new GapAnalysisRecurringJobHandler(NullLogger<GapAnalysisRecurringJobHandler>.Instance,
                                                      _mediator,
                                                      _context);
    }
    
    /// <summary>
    /// CASE 01: for each event type from GetEventTypesForAnalysis CalculateGapForEventType is called and its result is saved 
    /// </summary>
    [Fact]
    public async Task Given_EventTypes_Should_Calculate_Analysis_For_Each() {
        var eventType1 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var eventType2 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #2");
        _mediator.Send(Arg.Any<GetEventTypesForAnalysis>(), Arg.Any<CancellationToken>())
                 .Returns(new List<EventTypeAnalysisSettings>() {
                      new (eventType1.Id, _settings),
                      new (eventType2.Id, _settings)
                  });

        await _handler.Handle(_job, CancellationToken.None);

        await _mediator.Received(1)
                       .Send(Arg.Is<CalculateGapForEventType>(x => x.EventTypeId == eventType1.Id),
                             Arg.Any<CancellationToken>());
        await _mediator.Received(1)
                       .Send(Arg.Is<CalculateGapForEventType>(x => x.EventTypeId == eventType2.Id),
                             Arg.Any<CancellationToken>());
    }
    
    /// <summary>
    /// CASE 02: when CalculateGapForEventType return EventCount == 0, then SaveEventTypeAnalysisResult should be called with ResultJson = null
    /// </summary>
    [Fact]
    public async Task Given_Empty_Analysis_Result_Should_Save_It_As_Null() {
        var eventType1 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        _mediator.Send(Arg.Any<GetEventTypesForAnalysis>(), Arg.Any<CancellationToken>())
                 .Returns(new List<EventTypeAnalysisSettings>() {
                      new (eventType1.Id, _settings),
                  });
        _mediator.Send(Arg.Any<CalculateGapForEventType>(),
                       Arg.Any<CancellationToken>())
                 .Returns(new GapAnalysisResult(null, null, null, null, null, 0, 0));
        
        await _handler.Handle(_job, CancellationToken.None);
        
        await _mediator.Received(1)
                       .Send(Arg.Is<SaveEventTypeAnalysisResult>(x => x.EventTypeId == eventType1.Id && 
                                                                      x.ResultJson == null),
                             Arg.Any<CancellationToken>());
    }
}