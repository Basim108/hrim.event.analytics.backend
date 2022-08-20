using Hrim.Event.Analytics.Abstractions.Entities;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs; 

/// <summary> Soft deletion an instance of any entity type </summary>
/// <param name="Id">Entity id</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="CorrelationId"><see cref="BaseRequest"/></param>
/// <returns> Code(404) NotFound; EntityIsDeleted; Ok </returns>
public record SoftDeleteEntityCommand<TEntity>(Guid Id, bool SaveChanges, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<CqrsResult<TEntity?>>
    where TEntity: Entity, new();
    