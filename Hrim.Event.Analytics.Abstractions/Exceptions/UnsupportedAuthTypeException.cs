using System.Runtime.Serialization;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnsupportedAuthTypeException: Exception
{
    public UnsupportedAuthTypeException(string? authType)
        : base(CoreLogs.UNSUPPORTED_AUTH_TYPE + Sanitize(authType: authType)) {
        AuthenticationType = Sanitize(authType: authType);
    }

    protected UnsupportedAuthTypeException(SerializationInfo info, StreamingContext context)
        : base(info: info, context: context) {
        AuthenticationType = info.GetString(nameof(AuthenticationType));
    }

    /// <summary>
    ///     Name of the unsupported authentication type
    /// </summary>
    public string? AuthenticationType { get; }

    private static string Sanitize(string? authType) { return string.IsNullOrWhiteSpace(value: authType) ? "null or white space" : authType; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info: info, context: context);
        info.AddValue(nameof(AuthenticationType), AuthenticationType!);
    }
}