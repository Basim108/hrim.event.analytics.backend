using FluentValidation;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Analysis;

/// <summary> Validates analysis by event-type </summary>
public class AnalysisByEventTypeValidator: AbstractValidator<AnalysisConfigByEventType>
{
    /// <inheritdoc />
    public AnalysisByEventTypeValidator() {
        RuleFor(x => x.AnalysisCode)
           .Must(code => FeatureCodes.AllCodes.Contains(code))
           .WithMessage(ValidationMessages.UNSUPPORTED_ANALYSIS_CODE);
        RuleForEach(x => x.Settings)
           .Must(pair => AnalysisSettingNames.Gap.AllProps.Contains(pair.Key))
           .WithMessage(string.Format(ValidationMessages.UNSUPPORTED_ANALYSIS_SETTING, FeatureCodes.GAP_ANALYSIS))
           .Must(pair => !string.IsNullOrWhiteSpace(pair.Value))
           .WithMessage(ValidationMessages.IS_REQUIRED)
           .Must(pair => pair.Value.Length < 128)
           .WithMessage(ValidationMessages.TOO_LONG + 128)
           .When(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS);
        RuleFor(x => x.Settings)
           .Must(settings => settings.IsNullOrEmpty())
           .WithMessage(ValidationMessages.ANALYSIS_SHOULD_HAVE_NO_SETTINGS)
           .When(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS);
    }
}