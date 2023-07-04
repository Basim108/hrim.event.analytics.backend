using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.JobWorker.JobRunners;

public class ServiceProviderJobActivator: JobActivator
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderJobActivator(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public override object ActivateJob(Type jobType) {
        return _serviceProvider.GetRequiredService(jobType);
    }
}