using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Analysis.Cqrs;

public record EventTypeAnalysisSettings(long EventTypeId, IDictionary<string, string>? Settings, DateTime UpdatedAt, IEnumerable<long> ChildrenIds);

public record GetEventTypesForAnalysis(string AnalysisCode): IRequest<List<EventTypeAnalysisSettings>>;

public class GetEventTypesForAnalysisHandler: IRequestHandler<GetEventTypesForAnalysis, List<EventTypeAnalysisSettings>>
{
    private readonly EventAnalyticDbContext                   _context;
    private readonly ILogger<GetEventTypesForAnalysisHandler> _logger;

    public GetEventTypesForAnalysisHandler(EventAnalyticDbContext                   context,
                                           ILogger<GetEventTypesForAnalysisHandler> logger) {
        _context = context;
        _logger  = logger;
    }

    public async Task<List<EventTypeAnalysisSettings>> Handle(GetEventTypesForAnalysis request, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(request.AnalysisCode))
            throw new ArgumentNullException(nameof(request), nameof(request.AnalysisCode));

        var feature = await _context.HrimFeatures.FirstOrDefaultAsync(x => x.Code == request.AnalysisCode, cancellationToken);
        if (feature == null) {
            _logger.LogCritical(CoreLogs.FEATURE_IS_NOT_FOUND, request.AnalysisCode);
            throw new ArgumentOutOfRangeException(nameof(request), CoreLogs.FEATURE_IS_NOT_FOUND);
        }
        // due to refactoring - uncomment and check later
        return null;
        // return feature.IsOn
        //            ? await _context.AnalysisByEventType
        //                            .Include(x => x.EventType)
        //                            .ThenInclude(x => x!.Children)  
        //                            .Where(x => x.AnalysisCode == request.AnalysisCode && x.IsOn)
        //                            .Select(x => new EventTypeAnalysisSettings(x.EventTypeId, x.Settings, x.UpdatedAt, x.EventType!.Children!.Select(c => c.Id)))
        //                            .ToListAsync(cancellationToken)
        //            : new List<EventTypeAnalysisSettings>();
    }
}