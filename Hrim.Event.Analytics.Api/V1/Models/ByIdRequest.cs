using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Models;

public class ByIdRequest<TKey> where TKey: struct
{
    [FromRoute(Name = "id")] public TKey Id { get; set; }
}