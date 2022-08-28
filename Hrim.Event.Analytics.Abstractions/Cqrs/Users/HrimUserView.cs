namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users; 

/// <summary> User information for UI </summary>
public record HrimUserView(string Id, string FullName, string PictureUri);