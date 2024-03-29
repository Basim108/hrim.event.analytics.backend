using Hrim.Event.Analytics.Abstractions.Entities;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Entity;

/// <summary> Soft deletion an instance of any entity type </summary>
/// <param name="Id">Entity id</param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
/// <returns> Code(404) NotFound; EntityIsDeleted; Ok </returns>
public record SoftDeleteLongEntityCommand<TEntity>(long Id, bool SaveChanges, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<CqrsResult<TEntity?>>
    where TEntity : HrimEntity<long>, new();