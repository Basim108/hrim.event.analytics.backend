using System.Runtime.Serialization;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnsupportedEntityException : Exception
{
    public UnsupportedEntityException(Type entityType)
        : base(CoreLogs.UNSUPPORTED_ENTITY + entityType.FullName)
    {
        EntityType = entityType;
    }

    protected UnsupportedEntityException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        EntityType = info.GetValue(nameof(EntityType), typeof(Type)) as Type;
    }

    public Type? EntityType { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info, context);
        info.AddValue(nameof(EntityType), EntityType!);
    }
}