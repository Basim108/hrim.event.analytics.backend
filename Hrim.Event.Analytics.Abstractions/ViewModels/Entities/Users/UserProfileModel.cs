using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;

/// <summary>
/// User profile registration request
/// </summary>
public class UserProfileModel
{
    /// <summary> </summary>
    public string? FullName { get; set; }

    /// <summary> </summary>
    public string? FirstName { get; set; }

    /// <summary> </summary>
    public string? LastName { get; set; }
}