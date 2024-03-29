using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrimsoft.Core.Extensions;
using MediatR;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Users;

public class HrimUserCreateHandler: IRequestHandler<HrimUserCreateCommand, HrimUser>
{
    private readonly EventAnalyticDbContext _context;

    public HrimUserCreateHandler(EventAnalyticDbContext context) { _context = context; }

    public async Task<HrimUser> Handle(HrimUserCreateCommand request, CancellationToken cancellationToken) {
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var result = new HrimUser {
            CreatedAt       = now,
            UpdatedAt       = now,
            ConcurrentToken = 1
        };
        _context.HrimUsers.Add(entity: result);
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        return result;
    }
}