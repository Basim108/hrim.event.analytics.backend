#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions;

public static class CoreLogs {
    public const string HRIM_ENTITY_ID              = "HrimEntityId={HrimEntityId}";
    public const string UNEXPECTED_CQRS_RESULT_CODE = "Unexpected CqrsResultCode={CqrsResultCode}";
    public const string UNEXPECTED_CQRS_RESULT      = "Unexpected CqrsResult={CqrsResult}";
    public const string UNSUPPORTED_ENTITY          = "Unsupported entity of type: ";
    public const string FINISH_HANDLING             = "Finish handling";
    public const string START_HANDLING              = "Start handling";
    public const string CQRS_COMMAND                = "CqrsCommand={CqrsCommand}";
    public const string USER_ID                     = "UserId={UserId}";
    public const string CORRELATION_ID              = "CorrelationId={CorrelationId}";
}