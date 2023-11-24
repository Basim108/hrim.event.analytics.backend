using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Analysis;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class GetAnalysisByEventTypeIdHandler: IRequestHandler<GetAnalysisByEventTypeId, CqrsResult<List<AnalysisConfigByEventType>?>>
{
    private readonly EventAnalyticDbContext                   _context;
    private readonly ILogger<GetAnalysisByEventTypeIdHandler> _logger;
    private readonly IMediator                                _mediator;
    private readonly IMapper                                  _mapper;
    private readonly IApiRequestAccessor                      _requestAccessor;

    public GetAnalysisByEventTypeIdHandler(EventAnalyticDbContext                   context,
                                           ILogger<GetAnalysisByEventTypeIdHandler> logger,
                                           IMediator                                mediator,
                                           IMapper                                  mapper,
                                           IApiRequestAccessor                      requestAccessor) {
        _context         = context;
        _logger          = logger;
        _mediator        = mediator;
        _mapper          = mapper;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<List<AnalysisConfigByEventType>?>> Handle(GetAnalysisByEventTypeId request, CancellationToken cancellationToken) {
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request), nameof(request.EventTypeId));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<List<AnalysisConfigByEventType>?>> HandleAsync(GetAnalysisByEventTypeId request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.EventTypeId);
        try {
            var result = await _context.EventTypes.FirstOrDefaultAsync(x => x.Id == request.EventTypeId, cancellationToken);
            if (result == null)
                return new CqrsResult<List<AnalysisConfigByEventType>?>(Result: null, StatusCode: CqrsResultCode.NotFound);
            var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
            if (result.CreatedById != operatorUserId) {
                _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Read, result.CreatedById, nameof(AnalysisConfigByEventType));
                return new CqrsResult<List<AnalysisConfigByEventType>?>(Result: null, StatusCode: CqrsResultCode.Forbidden);
            }
            var query =
                from analysis in _context.AnalysisByEventType
                join feature in _context.HrimFeatures on analysis.AnalysisCode equals feature.Code
                where analysis.EventTypeId == request.EventTypeId && feature.IsOn
                select analysis;
            var settings = await query.AsNoTracking()
                                      .ProjectTo<AnalysisConfigByEventType>(_mapper.ConfigurationProvider)
                                      .ToListAsync(cancellationToken);
            var syncCommand    = new SyncAnalysisSettings(request.EventTypeId, settings, null, IsSaveChanges: true);
            var missedSettings = await _mediator.Send(syncCommand, cancellationToken);
            if (missedSettings is {
                    Count: > 0
                }) {
                if (_logger.IsEnabled(LogLevel.Debug)) {
                    var missedCodes = string.Join(", ", missedSettings.Select(x => x.AnalysisCode));
                    _logger.LogDebug(EfCoreLogs.PROCESSED_EVENT_TYPE, request.EventTypeId, missedCodes);
                }
                settings.AddRange(missedSettings);
            }
            return new CqrsResult<List<AnalysisConfigByEventType>?>(Result: settings, StatusCode: CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_TIMEOUT, HrimOperations.Read, ex.Message);
            return new CqrsResult<List<AnalysisConfigByEventType>?>(Result: null, StatusCode: CqrsResultCode.Locked);
        }
    }
}