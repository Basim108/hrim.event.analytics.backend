using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Features;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Manage analysis endpoints </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route(template: "v1/[controller]")]
public class AnalysisController: EventAnalyticsApiController<List<AnalysisConfigByEventType>>
{
    private readonly IAnalysisSettingsFactory _analysisSettings;
    private readonly IMediator                _mediator;

    /// <inheritdoc />
    public AnalysisController(IApiRequestAccessor      requestAccessor,
                              IAnalysisSettingsFactory analysisSettings,
                              IMediator                mediator)
        : base(requestAccessor) {
        _analysisSettings = analysisSettings;
        _mediator         = mediator;
    }

    /// <summary> Get all enabled analysis </summary>
    [HttpGet]
    public Task<List<AvailableAnalysis>> GetAllAsync(CancellationToken cancellationToken)
        => _mediator.Send(new GetAvailableAnalysisQuery(), cancellationToken: cancellationToken);

    /// <summary> Get analysis info of a particular event type </summary>
    [HttpGet("event-type/{eventTypeId}")]
    public async Task<ActionResult<List<AnalysisConfigByEventType>>> GetForEventType(long eventTypeId, CancellationToken cancellationToken) {
        if (eventTypeId == default)
            return _analysisSettings.GetDefaultSettings();
        var result = await _mediator.Send(new GetAnalysisByEventTypeId(eventTypeId, OperationContext),
                                          cancellationToken);
        return ProcessCqrsResult(cqrsResult: result);
    }


    /// <summary>
    /// Create or update analysis info for a particular event type 
    /// </summary>
    /// <returns>Returns updated analysis info</returns>
    [HttpPost("event-type/{eventTypeId}")]
    public async Task<ActionResult<List<AnalysisConfigByEventType>>> UpdateSettingsForEventType(long                            eventTypeId,
                                                                                                List<AnalysisConfigByEventType> analysis,
                                                                                                CancellationToken               cancellationToken) {
        var result = await _mediator.Send(new UpdateAnalysisForEventType(eventTypeId, analysis, OperationContext),
                                          cancellationToken);
        return ProcessCqrsResult(cqrsResult: result);
    }
}