using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.V1.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.ControllerTests;

[ExcludeFromCodeCoverage]
public class EventAnalyticsApiControllerTests
{
    private readonly EventAnalyticsApiController<UserEventType> _controller;

    public EventAnalyticsApiControllerTests() {
        _controller = new EventAnalyticsApiController<UserEventType>(Substitute.For<IApiRequestAccessor>());
    }

    private MethodInfo GetProcessCqrsResult() {
        return _controller.GetType()
                          .GetMethod(name: "ProcessCqrsResult", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    [Fact]
    public void ProcessCqrsResult_Given_EntityIsDeleted_Returns_EmptyResult() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.EntityIsDeleted);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Value.Should().BeNull();
    }

    [Fact]
    public void ProcessCqrsResult_Given_EntityIsDeleted_Returns_Gone() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.EntityIsDeleted);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Result.Should().BeOfType<StatusCodeResult>();
        var statusCodeResult = actionResult.Result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.Gone);
    }

    [Fact]
    public void ProcessCqrsResult_Given_Conflict_Without_Info_Returns_EmptyResult() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.Conflict);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Value.Should().BeNull();

        actionResult.Result.Should().BeOfType<StatusCodeResult>();
        var statusCodeResult = actionResult.Result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public void ProcessCqrsResult_Given_Conflict_With_Info_Returns_Info() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.Conflict, Info: "hello");
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Result.Should().BeOfType<ObjectResult>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.Value.Should().Be(JsonConvert.SerializeObject(value: cqrsResult.Info));
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public void ProcessCqrsResult_Given_Forbidden_Returns_Constant_Result() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.Forbidden);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Result.Should().BeOfType<ObjectResult>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.Value.Should().Be(expected: ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
    }

    [Fact]
    public void ProcessCqrsResult_Given_Locked_Returns_EmptyResult() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.Locked);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Value.Should().BeNull();

        actionResult.Result.Should().BeOfType<StatusCodeResult>();
        var statusCodeResult = actionResult.Result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.Locked);
    }

    [Fact]
    public void ProcessCqrsResult_Given_NotFound_Returns_EmptyResult() {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: CqrsResultCode.NotFound);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Value.Should().BeNull();

        actionResult.Result.Should().BeOfType<NotFoundResult>();
        var noteFoundResult = actionResult.Result as NotFoundResult;
        noteFoundResult.Should().NotBeNull();
        noteFoundResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(CqrsResultCode.Created)]
    [InlineData(CqrsResultCode.Ok)]
    public void ProcessCqrsResult_Given_Created_Or_Updated_Returns_OkResult(CqrsResultCode cqrsCode) {
        var dynMethod  = GetProcessCqrsResult();
        var cqrsResult = new CqrsResult<UserEventType?>(new UserEventType(), StatusCode: cqrsCode);
        var actionResult = dynMethod.Invoke(obj: _controller,
                                            new object[] {
                                                cqrsResult
                                            }) as ActionResult<UserEventType>;
        actionResult.Should().NotBeNull();
        actionResult!.Result.Should().BeOfType<OkObjectResult>();
        var objectResult = actionResult.Result as OkObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.Value.Should().NotBeNull();
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }
}