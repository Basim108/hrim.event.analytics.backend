using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class FeaturesData
{
    private readonly EventAnalyticDbContext _context;

    public FeaturesData(EventAnalyticDbContext context) { _context = context; }

    public HrimFeature EnsureExistence(string      varName,
                                       string      code,
                                       bool        isOn,
                                       string      description = "",
                                       FeatureType featureType = FeatureType.Analysis,
                                       bool        isDeleted   = false) {
        var feature = new HrimFeature {
            Id           = Guid.NewGuid(),
            VariableName = varName,
            Code         = code,
            IsOn         = isOn,
            FeatureType  = featureType,
            Description  = description,
            IsDeleted    = isDeleted
        };
        _context.HrimFeatures.Add(feature);
        _context.SaveChanges();
        return feature;
    }
}