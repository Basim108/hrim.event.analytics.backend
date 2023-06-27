using System.Diagnostics.CodeAnalysis;

namespace Hrim.Event.Analytics.Abstractions.Jobs.Configuration;

[ExcludeFromCodeCoverage]
public class HrimRecurringJobOptions
{
    public string   CronExpression { get; set; }
    public string   DisplayName    { get; set; }
    public string   JobId          { get; set; }
    public string   Queue          { get; set; }
}