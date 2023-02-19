#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Abstractions.Enums;

public enum CqrsResultCode
{
    Ok = 200,
    Created = 201,
    BadRequest = 400,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    PreConditionFailed = 412,
    UnprocessableEntity = 422,
    Locked = 423,
    EntityIsDeleted = 460,
    EntityIsNotDeleted = 461
}