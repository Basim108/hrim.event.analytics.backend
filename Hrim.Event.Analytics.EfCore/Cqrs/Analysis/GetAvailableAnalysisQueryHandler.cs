using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.EfCore.Cqrs.Analysis;

public class GetAvailableAnalysisQueryHandler: IRequestHandler<GetAvailableAnalysisQuery, List<AvailableAnalysis>>
{
    private readonly EventAnalyticDbContext _context;

    public GetAvailableAnalysisQueryHandler(EventAnalyticDbContext context) { _context = context; }

    /// <inheritdoc />
    public async Task<List<AvailableAnalysis>> Handle(GetAvailableAnalysisQuery request, CancellationToken cancellationToken) {
        return await _context.HrimFeatures
                             .AsNoTracking()
                             .Where(x => x.FeatureType == FeatureType.Analysis && x.IsOn)
                             .Select(x => new AvailableAnalysis {
                                  Code        = x.Code,
                                  Description = x.Description
                              })
                             .ToListAsync(cancellationToken);
    }
}