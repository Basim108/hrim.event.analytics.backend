using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Models;

public class ByIdRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; set; }
}