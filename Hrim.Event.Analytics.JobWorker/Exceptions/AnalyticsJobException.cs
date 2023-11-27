using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Hrim.Event.Analytics.JobWorker.Exceptions;

[ExcludeFromCodeCoverage]
[Serializable]
public class AnalyticsJobException: Exception
{
    public AnalyticsJobException(string message): base(message) { }    
    public AnalyticsJobException(string message, Exception innerException): base(message, innerException) { }
    public AnalyticsJobException(string cqrsCmd, string message, Exception innerException): base(message, innerException) {
        CqrsCommand = cqrsCmd;
    }   
    
    protected AnalyticsJobException(SerializationInfo info, StreamingContext context)
        : base(info: info, context: context) {
        CqrsCommand = info.GetString(nameof(CqrsCommand)) ?? "";
    }

    public string CqrsCommand { get; } = "";
    
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info: info, context: context);
        info.AddValue(nameof(CqrsCommand), CqrsCommand);
    }
}