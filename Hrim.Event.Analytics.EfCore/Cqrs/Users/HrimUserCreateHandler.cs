using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrimsoft.Core.Extensions;
using MediatR;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Users;

public class HrimUserCreateHandler: IRequestHandler<HrimUserCreateCommand, HrimUser> {
    private readonly EventAnalyticDbContext _context;

    public HrimUserCreateHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<HrimUser> Handle(HrimUserCreateCommand request, CancellationToken cancellationToken) {
        var result = new HrimUser {
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.HrimUsers.Add(result);
        if (request.SaveChanges) {
            await _context.SaveChangesAsync(cancellationToken);
        }
        return result;
    }
}