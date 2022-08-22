using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.ModelBinders;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Models;

public class EntityRequest: ByIdRequest {
    [FromQuery(Name = "entityType")]
    [ModelBinder(typeof(JsonModelBinder<EntityType>))]
    public EntityType EntityType { get; set; }
}