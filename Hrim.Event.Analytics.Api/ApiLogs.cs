#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api;

public static class ApiLogs {
    public const string FORBID_AS_NOT_ENTITY_OWNER = "Operation is forbidden as you are not an owner of the entity.";

    public const string JSON_MODEL_BINDER_DESERIALIZATION_ERROR =
        "Json deserialization of PropertyName={PropertyName},  ExceptionMessage={ExceptionMessage}, StackTrace={StackTrace}";
}