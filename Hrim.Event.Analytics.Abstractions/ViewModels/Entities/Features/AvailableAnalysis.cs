namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Features;

/// <summary>
/// Model to present information about an available analysis feature
/// </summary>
public class AvailableAnalysis
{
    /// <summary> Analysis Code </summary>
    public string Code        { get; set; }=string.Empty;
    
    /// <summary> Analysis Description </summary>
    public string? Description { get; set; }
}