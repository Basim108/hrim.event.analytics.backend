using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetAllTests : BaseCqrsTests
{
    [Fact]
    public async Task Given_IncludeOthersPublic_False_Returns_OnlyMine_Private_And_Public_EventTypes()
    {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        var myEvents = TestData.Events.CreateManyEventTypes(4, OperatorContext.UserId);
        myEvents.First().Value.IsPublic = false;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(1, anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(OperatorContext, false));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(4);
        resultList.All(x => myEvents.ContainsKey(x.Id)).Should().BeTrue();
        resultList.All(x => !anotherEventIds.Contains(x.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Given_IncludeOthersPublic_True_Returns_All_Mine_And_Public_Others()
    {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        var myEvents = TestData.Events.CreateManyEventTypes(4, OperatorContext.UserId);
        myEvents.First().Value.IsPublic = false;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(1, anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(OperatorContext, true));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(5);
        myEvents.Keys.All(myEventId => resultList.Any(resultEvent => myEventId == resultEvent.Id)).Should().BeTrue();
        resultList.Any(x => anotherEventIds.Contains(x.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Given_IncludeOthersPublic_True_Returns_Correct_IsMine_Property_Value()
    {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        var myEvents = TestData.Events.CreateManyEventTypes(4, OperatorContext.UserId);
        myEvents.First().Value.IsPublic = false;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(1, anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(OperatorContext, true));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(5);
        foreach (var myEventId in myEvents.Keys)
        {
            var resultEventType = resultList.First(x => x.Id == myEventId);
            resultEventType.IsMine.Should().BeTrue();
        }

        resultList.First(x => anotherEventIds.Contains(x.Id)).IsMine.Should().BeFalse();
    }
}