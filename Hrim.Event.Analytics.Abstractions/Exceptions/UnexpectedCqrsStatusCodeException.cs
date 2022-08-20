using System.Runtime.Serialization;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Enums;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnexpectedCqrsStatusCodeException: Exception {
    public CqrsResultCode? StatusCode { get; }

    public UnexpectedCqrsStatusCodeException(CqrsResultCode? statusCode)
        : base(CoreLogs.UnexpectedCqrsResult) {
        StatusCode = statusCode;
    }

    protected UnexpectedCqrsStatusCodeException(SerializationInfo info, StreamingContext context)
        : base(info, context) {
        StatusCode = (CqrsResultCode?)info.GetValue(nameof(StatusCode), typeof(CqrsResultCode?));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info, context);
        info.AddValue(nameof(StatusCode), StatusCode!);
    }
}