using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Analysis;

public class AnalysisByEventTypeListValidator: AbstractValidator<List<AnalysisByEventType>>
{
    public AnalysisByEventTypeListValidator() {
        RuleForEach(analysis => analysis)
           .SetValidator(new AnalysisByEventTypeValidator());
    }
}