using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetAllTests: BaseCqrsTests {

    [Fact]
    public async Task GetAll_Returns_Owned_EventTypes() {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        var myEventIds      = TestData.Events.CreateManyEventTypes(4, OperatorContext.UserId).Keys;
        var anotherEventIds = TestData.Events.CreateManyEventTypes(1, anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(OperatorContext));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(4);
        resultList.All(x => myEventIds.Contains(x.Id)).Should().BeTrue();
        resultList.All(x => !anotherEventIds.Contains(x.Id)).Should().BeTrue();
    }
}