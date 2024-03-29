#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions;

public static class CoreLogs
{
    public const string HRIM_ENTITY_ID              = "HrimEntityId={HrimEntityId}";
    public const string UNEXPECTED_CQRS_RESULT_CODE = "Unexpected CqrsResultCode={CqrsResultCode}";
    public const string UNEXPECTED_CQRS_RESULT      = "Unexpected CqrsResult={CqrsResult}";
    public const string UNSUPPORTED_ENTITY          = "Unsupported entity of type: ";
    public const string UNSUPPORTED_AUTH_TYPE       = "Unsupported authentication type: ";
    public const string UNSUPPORTED_ANALYSIS        = "Unsupported analysis code: ";
    public const string FINISH_HANDLING             = "Finish handling";
    public const string START_HANDLING              = "Start handling";
    public const string CQRS_COMMAND                = "CqrsCommand={CqrsCommand}";
    public const string EVENT_TYPE_ID               = "EventTypeId={EventTypeId}";
    public const string CORRELATION_ID              = "CorrelationId={CorrelationId}";
    
    public const string FEATURE_IS_NOT_FOUND = "Feature is not found by AnalysisCode={AnalysisCode}";
    
    public const string ENV_VARIABLE_IS_EMPTY = "EnvironmentVariable={EnvironmentVariable} is empty";

    public const string ENTITY_WITH_PROPERTY_ALREADY_EXISTS =
        "Entity with the same value of '{0}' property is already exists.";

    public const string ENTITY_WITH_2_PROPERTIES_ALREADY_EXISTS =
        "Entity with the same values of '{0}', '{1}' properties is already exists.";
}