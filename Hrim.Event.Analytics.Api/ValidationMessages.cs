#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api;

public static class ValidationMessages
{
    public const string ENTITY_DOES_NOT_EXISTS = "entity does not exists in the storage";
    public const string IS_REQUIRED = "is required";
    public const string GREATER_THAN_PROPERTY = $"must be greater than '{PROPERTY_NAME_TEMPLATE}'";

    public const string PROPERTY_NAME_TEMPLATE = "{AnotherPropertyName}";
}