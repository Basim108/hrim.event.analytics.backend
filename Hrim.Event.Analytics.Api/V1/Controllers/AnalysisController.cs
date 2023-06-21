using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Abstractions.Entities;
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
public class AnalysisController: EventAnalyticsApiController<HrimFeature>
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public AnalysisController(IApiRequestAccessor     requestAccessor,
                              IValidator<HrimFeature> validator,
                              IMediator               mediator)
        : base(requestAccessor, validator) {
        _mediator = mediator;
    }
    
    /// <summary> Get all user event types </summary>
    [HttpGet]
    public Task<IEnumerable<AvailableAnalysis>> GetAllAsync(CancellationToken cancellationToken) 
        => _mediator.Send(new GetAvailableAnalysisQuery(), cancellationToken: cancellationToken);
}