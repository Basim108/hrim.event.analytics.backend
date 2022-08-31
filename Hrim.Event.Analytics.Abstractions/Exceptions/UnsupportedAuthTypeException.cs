using System.Runtime.Serialization;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnsupportedAuthTypeException: Exception {
    /// <summary>
    /// Name of the unsupported authentication type
    /// </summary>
    public string? AuthenticationType { get; }

    public UnsupportedAuthTypeException(string? authType)
        : base(CoreLogs.UNSUPPORTED_AUTH_TYPE + authType) {
        AuthenticationType = Sanitize(authType);
    }
    
    private static string Sanitize(string? authType) 
        => string.IsNullOrWhiteSpace(authType) ? "null or white space" : authType;

    protected UnsupportedAuthTypeException(SerializationInfo info, StreamingContext context)
        : base(info, context) {
        AuthenticationType = info.GetString(nameof(AuthenticationType));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info, context);
        info.AddValue(nameof(AuthenticationType), AuthenticationType!);
    }
}