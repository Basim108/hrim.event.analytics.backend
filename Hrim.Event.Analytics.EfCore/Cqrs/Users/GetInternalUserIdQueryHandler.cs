using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Users;

public class GetInternalUserIdQueryHandler: IRequestHandler<GetInternalUserIdQuery, long>
{
    private readonly EventAnalyticDbContext                 _context;
    private readonly ILogger<GetInternalUserIdQueryHandler> _logger;

    public GetInternalUserIdQueryHandler(ILogger<GetInternalUserIdQueryHandler> logger,
                                         EventAnalyticDbContext                 context) {
        _logger  = logger;
        _context = context;
    }

    public Task<long> Handle(GetInternalUserIdQuery request, CancellationToken cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.Context == null)
            throw new ArgumentNullException(nameof(request), nameof(request.Context));

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<long> HandleAsync(GetInternalUserIdQuery request, CancellationToken cancellationToken) {
        var externalId = request.Context.ExternalId();
        var email      = request.Context.Email;

        IQueryable<ExternalUserProfile> query = _context.ExternalUserProfiles;
        query = string.IsNullOrWhiteSpace(value: email)
                    ? query.Where(x => x.ExternalUserId == externalId)
                    : query.Where(x => x.ExternalUserId == externalId || x.Email == email);

        var existedList = await query.Select(x => x.HrimUserId)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);
        if (existedList.Count > 1)
            _logger.LogWarning(message: EfCoreLogs.THERE_ARE_MANY_USERS_FOUND_BY_CLAIMS);
        return existedList.Count == 0
                   ? default
                   : existedList.First();
    }
}