using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Entity;

/// <summary> Restore any type of entity from soft deletion </summary>
/// <param name="Id">Entity Id</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="Context"><see cref="OperationRequest"/></param>
/// <returns> Code(404) NotFound; EntityIsNotDeleted; Ok </returns>
public record RestoreEntityCommand<TEntity>(Guid Id, bool SaveChanges, OperationContext Context)
    : OperationRequest(Context), IRequest<CqrsResult<TEntity?>>
    where TEntity: Entities.HrimEntity, new();