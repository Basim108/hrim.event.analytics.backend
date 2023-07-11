namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Analysis;

/// <summary> Result of a particular analysis </summary>
/// <param name="Code">Analysis code</param>
/// <param name="ResultJson">Calculation result serialized into JSON</param>
/// <param name="CalculatedAt">Date and UTC time when calculation happened</param>
public record ViewAnalysisResult(string Code, string? ResultJson, DateTime CalculatedAt);