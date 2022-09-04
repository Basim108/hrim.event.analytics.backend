using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.Cqrs.EventTypes;

[ExcludeFromCodeCoverage]
public class EventTypeGetAllTests: BaseCqrsTests {

    [Fact]
    public async Task GetAll_Returns_Owned_EventTypes() {
        var anotherUserId = Guid.NewGuid();
        TestData.CreateUser(anotherUserId);
        var myEventIds      = TestData.CreateManyEventTypes(4, OperatorContext.UserId).Keys;
        var anotherEventIds = TestData.CreateManyEventTypes(1, anotherUserId).Keys;

        var resultList = await Mediator.Send(new EventTypeGetAllMine(OperatorContext));
        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(4);
        resultList.All(x => myEventIds.Contains(x.Id)).Should().BeTrue();
        resultList.All(x => !anotherEventIds.Contains(x.Id)).Should().BeTrue();
    }
}