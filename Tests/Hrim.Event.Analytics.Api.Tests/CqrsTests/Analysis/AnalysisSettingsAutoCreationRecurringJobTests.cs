using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Hrim.Event.Analytics.Analysis.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis;

public class AnalysisSettingsAutoCreationRecurringJobTests
{
    private readonly AnalysisSettingsAutoCreationRecurringJobHandler _handler;
    private readonly AnalysisSettingsAutoCreationRecurringJob        _job      = new();
    private readonly IMediator                                       _mediator = Substitute.For<IMediator>();
    private readonly EventAnalyticDbContext                          _context  = DbUtils.GetDbContext();
    private readonly TestData                                        _testData;
    private readonly AnalysisByEventType                             _gapDefaultSettings;
    private readonly AnalysisByEventType                             _countDefaultSettings;

    public AnalysisSettingsAutoCreationRecurringJobTests() {
        _testData   = new TestData(_context);
        _testData.Features.EnsureExistence("FEAT_GAP", FeatureCodes.GAP_ANALYSIS, true);
        _testData.Features.EnsureExistence("FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true, "explanation");

        var settingsFactory = new AnalysisSettingsFactory();
        _gapDefaultSettings   = settingsFactory.GetDefaultSettings().First(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS);
        _countDefaultSettings = settingsFactory.GetDefaultSettings().First(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS);
        _handler = new AnalysisSettingsAutoCreationRecurringJobHandler(NullLogger<AnalysisSettingsAutoCreationRecurringJobHandler>.Instance,
                                                                       _mediator,
                                                                       _context);
    }
    
    [Fact]
    public async Task Given_3_EventTypes_When_1_Without_Any_Settings_Should_Sync_All_of_Them() {
        var eventType1 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var eventType2 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #2");
        var eventType3 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #3");
        _testData.AnalysisByEventType.EnsureExistence(eventType1.Id, _gapDefaultSettings);
        _testData.AnalysisByEventType.EnsureExistence(eventType1.Id, _countDefaultSettings);
        _testData.AnalysisByEventType.EnsureExistence(eventType3.Id, _gapDefaultSettings);
        _testData.AnalysisByEventType.EnsureExistence(eventType3.Id, _countDefaultSettings);

        await _handler.Handle(_job, CancellationToken.None);

        await _mediator.Received(1)
                       .Send(Arg.Is<SyncAnalysisSettings>(x => x.EventTypeId == eventType1.Id),
                             Arg.Any<CancellationToken>());
        await _mediator.Received(1)
                       .Send(Arg.Is<SyncAnalysisSettings>(x => x.EventTypeId == eventType2.Id),
                             Arg.Any<CancellationToken>());
        await _mediator.Received(1)
                       .Send(Arg.Is<SyncAnalysisSettings>(x => x.EventTypeId == eventType3.Id),
                             Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Given_3_EventTypes_When_1_IsDeleted_Should_Ignore_Deleted() {
        var eventType1 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var eventType2 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #2", isDeleted: true);
        var eventType3 = _testData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #3");

        await _handler.Handle(_job, CancellationToken.None);

        await _mediator.Received(1)
                       .Send(Arg.Is<SyncAnalysisSettings>(x => x.EventTypeId == eventType1.Id),
                             Arg.Any<CancellationToken>());
        await _mediator.Received(0)
                       .Send(Arg.Is<SyncAnalysisSettings>(x => x.EventTypeId == eventType2.Id),
                             Arg.Any<CancellationToken>());
        await _mediator.Received(1)
                       .Send(Arg.Is<SyncAnalysisSettings>(x => x.EventTypeId == eventType3.Id),
                             Arg.Any<CancellationToken>());
    }
}