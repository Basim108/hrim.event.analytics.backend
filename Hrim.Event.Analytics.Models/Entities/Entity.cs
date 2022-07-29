namespace Hrim.Event.Analytics.Models.Entities;

/// <summary> Each entity own these properties </summary>
/// <param name="Id">Entity id</param>
/// <param name="CreatedAt">Date and UTC time of event_type creation</param>
/// <param name="UpdateAt">Date and UTC time of event_type last update</param>
/// <param name="IsDeleted">Soft deletion flag</param>
/// <param name="ConcurrentToken">Update is possible only this token equals to the token in the storage</param>
public abstract record Entity(Guid      Id,
                              DateTime  CreatedAt,
                              DateTime? UpdateAt,
                              bool      IsDeleted,
                              int       ConcurrentToken);