namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary> Each entity own these properties </summary>
public abstract class HrimEntity<TKey> where TKey: struct
{
    /// <summary> Entity id </summary>
    public TKey Id { get; set; }

    /// <summary> Date and UTC time of entity instance creation </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary> Date and UTC time of entity instance last update </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary> Soft deletion flag </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    ///     Update is possible only when this token equals to the token in the storage
    /// </summary>
    public long ConcurrentToken { get; set; }

    /// <summary> copy all entity properties to the another entity </summary>
    // TODO: remove this cloning! switch AutoMapper
    public void CopyTo(HrimEntity<TKey> another) {
        another.Id              = Id;
        another.CreatedAt       = CreatedAt;
        another.UpdatedAt       = UpdatedAt;
        another.IsDeleted       = IsDeleted;
        another.ConcurrentToken = ConcurrentToken;
    }
}