#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api;

public static class ApiLogs {
    public const string FORBID_AS_NOT_ENTITY_OWNER    = "Operation is forbidden as you are not an owner of the entity.";
    public const string IDENTITY_IS_NOT_AUTHENTICATED = "Identity is not authenticated.";
    public const string FB_PICTURE_WAS_ADDED          = "Facebook picture claim was added.";
    public const string EXTERNAL_USER_ID              = "ExternalUserId={ExternalUserId}";
    public const string AUTH_TYPE                     = "AuthenticationType={AuthenticationType}";

    public const string JSON_MODEL_BINDER_DESERIALIZATION_ERROR =
        "Json deserialization of PropertyName={PropertyName},  ExceptionMessage={ExceptionMessage}, StackTrace={StackTrace}";
}