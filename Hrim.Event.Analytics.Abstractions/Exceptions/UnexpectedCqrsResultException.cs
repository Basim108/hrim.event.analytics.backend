using System.Runtime.Serialization;
using Hrim.Event.Analytics.Abstractions.Cqrs;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnexpectedCqrsResultException<TResult>: Exception {
    public CqrsResult<TResult>? CqrsResult { get; }

    public UnexpectedCqrsResultException(CqrsResult<TResult> cqrsResult)
        : base(CoreLogs.UnexpectedCqrsResult) {
        CqrsResult = cqrsResult;
    }

    protected UnexpectedCqrsResultException(SerializationInfo info, StreamingContext context)
        : base(info, context) {
        CqrsResult = info.GetValue(nameof(CqrsResult), typeof(Type)) as CqrsResult<TResult>;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info, context);
        info.AddValue(nameof(CqrsResult), CqrsResult!);
    }
}