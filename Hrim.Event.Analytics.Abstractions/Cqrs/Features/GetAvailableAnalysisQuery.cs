using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Features;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Features;

/// <summary>
/// Access a list of available (IsOn) analysis features
/// </summary>
public record GetAvailableAnalysisQuery(): IRequest<IEnumerable<AvailableAnalysis>>;