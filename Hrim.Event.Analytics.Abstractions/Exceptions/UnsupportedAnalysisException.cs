using System.Runtime.Serialization;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Exceptions;

[Serializable]
public class UnsupportedAnalysisException: Exception
{
    public UnsupportedAnalysisException(string analysisCode)
        : base(CoreLogs.UNSUPPORTED_ANALYSIS + analysisCode) {
        AnalysisCode = analysisCode;
    }

    protected UnsupportedAnalysisException(SerializationInfo info, StreamingContext context)
        : base(info: info, context: context) {
        AnalysisCode = info.GetString(nameof(AnalysisCode)) ?? string.Empty;
    }

    /// <summary> Code of unsupported analysis </summary>
    public string AnalysisCode { get; }


    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        if (info == null)
            throw new ArgumentNullException(nameof(info));
        base.GetObjectData(info: info, context: context);
        info.AddValue(nameof(AnalysisCode), AnalysisCode!);
    }
}