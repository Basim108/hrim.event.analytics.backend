using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Manage analysis endpoints </summary>
[ApiController]
[Authorize]
[Route(template: "v1/[controller]")]
public class AnalysisController: EventAnalyticsApiController<List<AnalysisByEventType>>
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public AnalysisController(IApiRequestAccessor requestAccessor,
                              IMediator           mediator)
        : base(requestAccessor) {
        _mediator = mediator;
    }

    /// <summary> Get all user event types </summary>
    [HttpGet]
    public Task<List<AvailableAnalysis>> GetAllAsync(CancellationToken cancellationToken)
        => _mediator.Send(new GetAvailableAnalysisQuery(), cancellationToken: cancellationToken);

    [HttpGet("/event-type/{eventTypeId}")]
    public async Task<ActionResult<List<AnalysisByEventType>>> GetForEventType(Guid eventTypeId, CancellationToken cancellationToken) {
        var result = await _mediator.Send(new GetAnalysisByEventTypeId(eventTypeId, OperationContext),
                                          cancellationToken);
        return ProcessCqrsResult(cqrsResult: result);
    }
}