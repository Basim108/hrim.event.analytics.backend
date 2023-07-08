using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;
using Hrim.Event.Analytics.Analysis.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.GapAnalysis;

[ExcludeFromCodeCoverage]
public class GapCalculationServiceTests
{
    private readonly GapSettings           _settings;
    private readonly GapCalculationService _service;

    public GapCalculationServiceTests() {
        _settings = new GapSettings(TimeSpan.FromDays(1));
        _service  = new GapCalculationService();
    }

    [Fact]
    public void Given_Empty_List_Should_Return_EventCount_0() {
        var list              = new List<AnalysisEvent>();
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.EventCount.Should().Be(0);
    }

    [Fact]
    public void Given_One_Event_Should_Return_No_Gaps_Result() {
        var list = new List<AnalysisEvent> {
            new(DateOnly.FromDateTime(DateTime.UtcNow),
                DateTimeOffset.UtcNow, null, null)
        };
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.GapCount.Should().Be(0);
        calculationResult.Avg.Should().BeNull();
        calculationResult.Min.Should().BeNull();
        calculationResult.MinGapDate.Should().BeNull();
        calculationResult.Max.Should().BeNull();
        calculationResult.MaxGapDate.Should().BeNull();
    }

    [Fact]
    public void Given_3_Events_When_Strongly_Less_MinimalGap_Should_Return_No_Gaps_Result() {
        var list = new List<AnalysisEvent> {
            new(DateOnly.FromDateTime(DateTime.UtcNow),
                DateTime.UtcNow.AddHours(-4),
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateTime.UtcNow.AddHours(-3)),
            new(DateOnly.FromDateTime(DateTime.UtcNow),
                DateTime.UtcNow.AddHours(-2),
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateTime.UtcNow.AddHours(-1)),
            new(DateOnly.FromDateTime(DateTime.UtcNow),
                DateTimeOffset.UtcNow,
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateTime.UtcNow.AddMinutes(30))
        };
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.GapCount.Should().Be(0);
        calculationResult.Avg.Should().BeNull();
        calculationResult.Min.Should().BeNull();
        calculationResult.MinGapDate.Should().BeNull();
        calculationResult.Max.Should().BeNull();
        calculationResult.MaxGapDate.Should().BeNull();
    }

    [Fact]
    public void Given_3_Events_When_Equal_MinimalGap_Should_Return_No_Gaps_Result() {
        var now = DateTime.UtcNow;
        var firstEvent = new AnalysisEvent(DateOnly.FromDateTime(now), now,
                                              DateOnly.FromDateTime(now), now);
        var secondEvent = new AnalysisEvent(firstEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartTime.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartTime.AddDays((int)_settings.MinimalGap.TotalDays));
        var thirdEvent = new AnalysisEvent(secondEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                              secondEvent.StartTime.Add(_settings.MinimalGap),
                                              secondEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                              secondEvent.StartTime.Add(_settings.MinimalGap));
        var list              = new List<AnalysisEvent> { firstEvent, secondEvent, thirdEvent };
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.GapCount.Should().Be(0);
        calculationResult.Avg.Should().BeNull();
        calculationResult.Min.Should().BeNull();
        calculationResult.MinGapDate.Should().BeNull();
        calculationResult.Max.Should().BeNull();
        calculationResult.MaxGapDate.Should().BeNull();
    }

    [Fact]
    public void Given_3_Events_When_1_Gap_Should_Return_Min_Equal_Max() {
        var now = DateTime.UtcNow;
        var firstEvent = new AnalysisEvent(DateOnly.FromDateTime(now), now,
                                              DateOnly.FromDateTime(now), now);
        var secondEvent = new AnalysisEvent(firstEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartTime.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartTime.AddDays((int)_settings.MinimalGap.TotalDays));
        var thirdEvent = new AnalysisEvent(secondEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                              secondEvent.StartTime.Add(_settings.MinimalGap).AddHours(1),
                                              secondEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                              secondEvent.StartTime.Add(_settings.MinimalGap).AddHours(1));
        var list              = new List<AnalysisEvent> { firstEvent, secondEvent, thirdEvent };
        var expectedGap       = TimeSpan.Parse("01:01:00:00");
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.GapCount.Should().Be(1);
        calculationResult.Avg.Should().Be(expectedGap);
        calculationResult.Min.Should().Be(expectedGap);
        calculationResult.MinGapDate.Should().Be(DateOnly.FromDateTime(now.Add(_settings.MinimalGap)));
        calculationResult.Max.Should().Be(expectedGap);
        calculationResult.MaxGapDate.Should().Be(DateOnly.FromDateTime(now.Add(_settings.MinimalGap)));
    }

    [Fact]
    public void Given_3_Events_When_2_Gap_Should_Return_Correct() {
        var now = DateTime.UtcNow;
        var firstEvent = new AnalysisEvent(DateOnly.FromDateTime(now), now,
                                              DateOnly.FromDateTime(now), now);
        var secondEvent = new AnalysisEvent(firstEvent.FinishDate!.Value.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.FinishTime!.Value.AddDays((int)_settings.MinimalGap.TotalDays).AddHours(1),
                                               firstEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartTime.AddDays((int)_settings.MinimalGap.TotalDays).AddHours(2));
        var thirdEvent = new AnalysisEvent(secondEvent.FinishDate!.Value.AddDays((int)_settings.MinimalGap.TotalDays + 1),
                                              secondEvent.FinishTime!.Value.Add(_settings.MinimalGap).AddHours(1),
                                              secondEvent.FinishDate.Value.AddDays((int)_settings.MinimalGap.TotalDays + 1),
                                              secondEvent.FinishTime.Value.Add(_settings.MinimalGap).AddHours(2));
        var list              = new List<AnalysisEvent> { firstEvent, secondEvent, thirdEvent };
        var expectedMinGap    = TimeSpan.Parse("01:01:00:00");
        var expectedMaxGap    = TimeSpan.Parse("02:01:00:00");
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.GapCount.Should().Be(2);
        calculationResult.Avg.Should().Be(TimeSpan.FromSeconds((expectedMinGap + expectedMaxGap).TotalSeconds / 2));
        calculationResult.Min.Should().Be(expectedMinGap);
        calculationResult.MinGapDate.Should().Be(DateOnly.FromDateTime(now));
        calculationResult.Max.Should().Be(expectedMaxGap);
        calculationResult.MaxGapDate.Should().Be(DateOnly.FromDateTime(now.Add(_settings.MinimalGap).AddHours(1)));
    }

    [Fact]
    public void Given_3_Events_When_1_Gap_Has_No_FinishedOn_Date_Should_Use_StartedOn() {
        var now = DateTime.UtcNow;
        var firstEvent = new AnalysisEvent(DateOnly.FromDateTime(now), now,
                                              DateOnly.FromDateTime(now), now);
        var secondEvent = new AnalysisEvent(firstEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                               firstEvent.StartTime.AddDays((int)_settings.MinimalGap.TotalDays),
                                               null, null);
        var thirdEvent = new AnalysisEvent(secondEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                              secondEvent.StartTime.Add(_settings.MinimalGap).AddHours(1),
                                              secondEvent.StartDate.AddDays((int)_settings.MinimalGap.TotalDays),
                                              secondEvent.StartTime.Add(_settings.MinimalGap).AddHours(1));
        var list              = new List<AnalysisEvent> { firstEvent, secondEvent, thirdEvent };
        var expectedGap       = TimeSpan.Parse("01:01:00:00");
        var calculationResult = _service.Calculate(list, _settings);
        calculationResult.Should().NotBeNull();
        calculationResult.GapCount.Should().Be(1);
        calculationResult.Avg.Should().Be(expectedGap);
        calculationResult.Min.Should().Be(expectedGap);
        calculationResult.MinGapDate.Should().Be(DateOnly.FromDateTime(now.Add(_settings.MinimalGap)));
        calculationResult.Max.Should().Be(expectedGap);
        calculationResult.MaxGapDate.Should().Be(DateOnly.FromDateTime(now.Add(_settings.MinimalGap)));
    }
}