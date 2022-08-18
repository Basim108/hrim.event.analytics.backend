namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary> Each entity own these properties </summary>
public abstract class Entity {
    /// <summary> Entity id </summary>
    public Guid Id { get; set; }

    /// <summary> Date and UTC time of entity instance creation </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary> Date and UTC time of entity instance last update </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary> Soft deletion flag </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Update is possible only when this token equals to the token in the storage
    /// </summary>
    public uint ConcurrentToken { get; set; }
}