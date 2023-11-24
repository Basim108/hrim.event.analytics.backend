using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetAllTests: BaseCqrsTests
{
    [Fact]
    public async Task Given_EventType_When_Not_Empty_AnalysisResults_Returns_Them() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId).Bl;
        var gapResult = TestData.AnalysisResults.EnsureGapExistence(eventType.Id);

        var resultList = await Mediator.Send(new EventTypeGetAllMine(Context: OperatorContext));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 1);
        resultList[0].AnalysisResults.Should().NotBeEmpty();
        var analysisResult = resultList[0].AnalysisResults!.First();
        analysisResult.Should().NotBeNull();
        analysisResult.Code.Should().Be(FeatureCodes.GAP_ANALYSIS);
        analysisResult.ResultJson.Should().NotBeNull();
        analysisResult.CalculatedAt.Should().Be(gapResult.FinishedAt);
    }

    [Fact]
    public async Task Given_IncludeOthersPublic_False_Returns_OnlyMine_Private_And_Public_EventTypes() {
        var anotherUserId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var myEvents = TestData.Events.CreateManyEventTypes(count: 4, userId: OperatorUserId);
        myEvents.First().Value.IsPublic = false;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(Context: OperatorContext));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 4);
        resultList.All(x => myEvents.ContainsKey(key: x.Id)).Should().BeTrue();
        resultList.All(x => !anotherEventIds.Contains(value: x.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Given_IncludeOthersPublic_True_Returns_All_Mine_And_Public_Others() {
        var anotherUserId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var myEvents = TestData.Events.CreateManyEventTypes(count: 4, userId: OperatorUserId);
        myEvents.First().Value.IsPublic = false;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(Context: OperatorContext, IncludeOthersPublic: true));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 5);
        myEvents.Keys.All(myEventId => resultList.Any(resultEvent => myEventId == resultEvent.Id)).Should().BeTrue();
        resultList.Any(x => anotherEventIds.Contains(value: x.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Given_IncludeOthersPublic_True_Returns_Correct_IsMine_Property_Value() {
        var anotherUserId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var myEvents = TestData.Events.CreateManyEventTypes(count: 4, userId: OperatorUserId);
        myEvents.First().Value.IsPublic = false;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(count: 1, userId: anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(Context: OperatorContext, IncludeOthersPublic: true));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 5);
        foreach (var myEventId in myEvents.Keys) {
            var resultEventType = resultList.First(x => x.Id == myEventId);
            resultEventType.IsMine.Should().BeTrue();
        }

        resultList.First(x => anotherEventIds.Contains(value: x.Id)).IsMine.Should().BeFalse();
    }
}