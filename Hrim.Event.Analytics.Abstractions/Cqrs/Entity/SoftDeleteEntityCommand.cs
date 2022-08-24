using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Entity; 

/// <summary> Soft deletion an instance of any entity type </summary>
/// <param name="Id">Entity id</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="Context"><see cref="OperationRequest"/></param>
/// <returns> Code(404) NotFound; EntityIsDeleted; Ok </returns>
public record SoftDeleteEntityCommand<TEntity>(Guid Id, bool SaveChanges, OperationContext Context)
    : OperationRequest(Context), IRequest<CqrsResult<TEntity?>>
    where TEntity: Entities.HrimEntity, new();
    