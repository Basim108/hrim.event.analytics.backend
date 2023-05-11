using System.Runtime.Serialization;
using Hrim.Event.Analytics.Abstractions.Cqrs;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnexpectedCqrsResultException<TResult>: Exception
{
    public UnexpectedCqrsResultException(CqrsResult<TResult> cqrsResult)
        : base(CoreLogs.UNEXPECTED_CQRS_RESULT.Replace(oldValue: "{CqrsResult}", cqrsResult.ToString())) {
        CqrsResult = cqrsResult;
    }

    protected UnexpectedCqrsResultException(SerializationInfo info, StreamingContext context)
        : base(info: info, context: context) {
        CqrsResult = info.GetValue(nameof(CqrsResult), typeof(CqrsResult<TResult>)) as CqrsResult<TResult>;
    }

    public CqrsResult<TResult>? CqrsResult { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info: info, context: context);
        info.AddValue(nameof(CqrsResult), CqrsResult!);
    }
}