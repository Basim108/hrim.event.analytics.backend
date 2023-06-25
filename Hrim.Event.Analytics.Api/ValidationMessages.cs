#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api;

public static class ValidationMessages
{
    public const string ENTITY_DOES_NOT_EXISTS = "entity does not exists in the storage";
    public const string IS_REQUIRED            = "is required";
    public const string TOO_LONG               = "is too long. must be less then ";
    public const string GREATER_THAN_PROPERTY  = $"must be greater than '{PROPERTY_NAME_TEMPLATE}'";

    public const string UNSUPPORTED_ANALYSIS_CODE        = "Unsupported analysis code";
    public const string UNSUPPORTED_ANALYSIS_SETTING     = "Unsupported {0} analysis setting";
    public const string ANALYSIS_SHOULD_HAVE_NO_SETTINGS = "Analysis should have no settings";

    public const string PROPERTY_NAME_TEMPLATE = "{AnotherPropertyName}";
}