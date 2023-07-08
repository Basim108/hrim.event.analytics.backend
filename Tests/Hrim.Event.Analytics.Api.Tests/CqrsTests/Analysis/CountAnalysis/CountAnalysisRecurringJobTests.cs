using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis.Models;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.CountAnalysis;

public class CountAnalysisRecurringJobTests
{
    private readonly CountAnalysisRecurringJobHandler _handler;
    private readonly CountAnalysisRecurringJob        _job      = new(Guid.NewGuid());
    private readonly IMediator                        _mediator = Substitute.For<IMediator>();
    private readonly EventAnalyticDbContext           _context  = DbUtils.GetDbContext();
    private readonly TestData                         _testData;

    public CountAnalysisRecurringJobTests() {
        _testData = new TestData(_context);
        _handler = new CountAnalysisRecurringJobHandler(NullLogger<CountAnalysisRecurringJobHandler>.Instance,
                                                        _mediator,
                                                        _context);
    }
    
    /// <summary>
    /// CASE 01: for each event type from GetEventTypesForAnalysis CalculateCountForEventType is called and its result is saved 
    /// </summary>
    [Fact]
    public async Task Given_EventTypes_Should_Calculate_Analysis_For_Each() {
        var eventType1 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var eventType2 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #2");
        _mediator.Send(Arg.Any<GetEventTypesForAnalysis>(), Arg.Any<CancellationToken>())
                 .Returns(new List<EventTypeAnalysisSettings>() {
                      new (eventType1.Id, null),
                      new (eventType2.Id, null)
                  });

        await _handler.Handle(_job, CancellationToken.None);

        await _mediator.Received(1)
                       .Send(Arg.Is<CalculateCountForEventType>(x => x.EventTypeId == eventType1.Id),
                             Arg.Any<CancellationToken>());
        await _mediator.Received(1)
                       .Send(Arg.Is<CalculateCountForEventType>(x => x.EventTypeId == eventType2.Id),
                             Arg.Any<CancellationToken>());
    }
    
    /// <summary>
    /// CASE 02: when CalculateCountForEventType return both duration and occurrence count == 0, then SaveEventTypeAnalysisResult should be called with ResultJson = null
    /// </summary>
    [Fact]
    public async Task Given_Empty_Analysis_Result_Should_Save_It_As_Null() {
        var eventType1 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        _mediator.Send(Arg.Any<GetEventTypesForAnalysis>(), Arg.Any<CancellationToken>())
                 .Returns(new List<EventTypeAnalysisSettings>() {
                      new (eventType1.Id, null),
                  });
        _mediator.Send(Arg.Any<CalculateCountForEventType>(),
                       Arg.Any<CancellationToken>())
                 .Returns(new CountAnalysisResult(null, null, null, null, null, null, 0, 0));
        
        await _handler.Handle(_job, CancellationToken.None);
        
        await _mediator.Received(1)
                       .Send(Arg.Is<SaveEventTypeAnalysisResult>(x => x.EventTypeId == eventType1.Id && 
                                                                      x.ResultJson == null),
                             Arg.Any<CancellationToken>());
    }
}