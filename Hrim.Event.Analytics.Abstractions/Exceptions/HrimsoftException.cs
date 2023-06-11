#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

/// <summary> user exception </summary>
[Serializable]
public class HrimsoftException: Exception
{
    public HrimsoftException(string message):base(message) { }
    public HrimsoftException(string message, Exception innerException):base(message, innerException) { }
}