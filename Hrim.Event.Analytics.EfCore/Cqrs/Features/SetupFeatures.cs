using System;
using System.Threading;
using System.Threading.Tasks;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Features;

public class SetupFeaturesHandler: IRequestHandler<SetupFeatures, Unit>
{
    private readonly EventAnalyticDbContext        _context;
    private readonly ILogger<SetupFeaturesHandler> _logger;
    private readonly IConfiguration                _appConfig;

    public SetupFeaturesHandler(EventAnalyticDbContext        context,
                                ILogger<SetupFeaturesHandler> logger,
                                IConfiguration                appConfig) {
        _context   = context;
        _logger    = logger;
        _appConfig = appConfig;
    }

    public async Task<Unit> Handle(SetupFeatures request, CancellationToken cancellationToken) {
        var featureList = await _context.HrimFeatures.ToListAsync(cancellationToken);
        var isChanged   = false;
        foreach (var feature in featureList) {
            var value = _appConfig[feature.VariableName];
            if (string.IsNullOrWhiteSpace(value)) {
                _logger.LogError(CoreLogs.ENV_VARIABLE_IS_EMPTY, feature.VariableName);
                continue;
            }
            var isOn = value.ToLowerInvariant() == "on";
            if (feature.IsOn == isOn) {
                continue;
            }
            feature.IsOn      = isOn;
            feature.UpdatedAt = DateTime.UtcNow;
            feature.ConcurrentToken++;
            isChanged = true;
        }
        if (isChanged) {
            await _context.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}