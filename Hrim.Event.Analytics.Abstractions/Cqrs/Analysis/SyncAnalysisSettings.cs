using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;

/// <summary>
/// Checks analysis settings of an event type and creates missing settings with default values
/// </summary>
/// <param name="EventTypeId"></param>
/// <param name="CurrentSettings">Existed settings for some of analysis, e.g. gap and/or count</param>
/// <param name="Features">List of all features available and not</param>
/// <param name="IsSaveChanges">If true will save changes after creation</param>
/// <returns>Returns missed settings and null in case no settings were missed</returns>
public record SyncAnalysisSettings(Guid                       EventTypeId,
                                   List<AnalysisByEventType>? CurrentSettings,
                                   List<HrimFeature>?         Features,
                                   bool                       IsSaveChanges)
    : IRequest<List<AnalysisByEventType>?>;