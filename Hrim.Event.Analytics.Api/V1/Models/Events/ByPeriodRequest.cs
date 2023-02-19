using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Models;

public class ByPeriodRequest
{
    [FromQuery(Name = "start")] public DateOnly Start { get; set; }

    [FromQuery(Name = "end")] public DateOnly End { get; set; }
}