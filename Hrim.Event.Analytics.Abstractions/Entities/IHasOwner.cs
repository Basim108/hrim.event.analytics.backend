namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary> Entities that has an owner  </summary>
public interface IHasOwner
{
    /// <summary>
    ///     User identifier of an owner
    /// </summary>
    long CreatedById { get; set; }
}