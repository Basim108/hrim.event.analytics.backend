using System.Runtime.Serialization;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnsupportedEntityException: Exception {
    public UnsupportedEntityException(Type entityType)
        : base(CoreLogs.UnsupportedEntity + entityType.FullName) {
        EntityType = entityType;
    }

    public Type? EntityType { get; }

    protected UnsupportedEntityException(SerializationInfo info, StreamingContext context)
        : base(info, context) {
        EntityType = info.GetValue(nameof(EntityType), typeof(Type)) as Type;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info, context);
        info.AddValue(nameof(EntityType), EntityType!);
    }
}