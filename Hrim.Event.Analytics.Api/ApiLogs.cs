#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api;

public static class ApiLogs
{
    public const string FORBID_AS_NOT_ENTITY_OWNER    = "Operation is forbidden as you are not an owner of the entity.";
    public const string IDENTITY_IS_NOT_AUTHENTICATED = "Identity is not authenticated.";
    public const string FB_PICTURE_WAS_ADDED          = "Facebook picture claim was added.";
    public const string EXTERNAL_USER_ID              = "ExternalUserId={ExternalUserId}";
    public const string INTERNAL_USER_ID              = "InternalUserId={InternalUserId}";
    public const string AUTH_TYPE                     = "AuthenticationType={AuthenticationType}";
    public const string REQUEST_HEADERS               = "RequestHeaders={RequestHeaders}";
    public const string REQUEST_BODY                  = "RequestBody={RequestBody}";
    public const string UNHANDLED_EXCEPTION           = "UnhandledException={UnhandledException}";
    public const string RESPONSE_STATUS_CODE          = "ResponseStatusCode={ResponseStatusCode}";
    public const string RETURN_URI_IS_NOT_ALLOWED     = "Provided return uri is not allowed";

    public const string RETURN_URI_IS_IN_WRONG_FORMAT =
        "Provided return uri has wrong format. ErrorMessage={ErrorMessage}";

    public const string RETURN_URI_PROCESSING_ERROR = "Error while processing return uri. ErrorMessage={ErrorMessage}";
    public const string SET_OPERATOR_ID_TO_ENTITY   = "Set OperatorId={OperatorId} for the ArgumentName={ArgumentName}";

    public const string NO_OWNER_SET_IN_FILTER =
        "There is no arguments that implements IHasOwner interface; filter is redundant in this case.";

    public const string JSON_MODEL_BINDER_DESERIALIZATION_ERROR =
        "Json deserialization of PropertyName={PropertyName},  ExceptionMessage={ExceptionMessage}, StackTrace={StackTrace}";
}