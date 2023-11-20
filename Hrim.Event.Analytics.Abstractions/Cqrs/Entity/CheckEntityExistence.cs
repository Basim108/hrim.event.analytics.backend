using Hrim.Event.Analytics.Abstractions.Enums;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Entity;

/// <summary> Checks the existence of entity in the storage </summary>
/// <param name="Id">Id of the entity that has to be checked</param>
/// <param name="EntityType">type of entity</param>
/// <param name="CorrelationId">
///     <see cref="BaseRequest" />
/// </param>
public record CheckEntityExistence(long Id, EntityType EntityType, Guid CorrelationId)
    : BaseRequest(CorrelationId: CorrelationId), IRequest<CqrsVoidResult>;

/// <summary> Checks the existence of a user in the storage </summary>
public record CheckUserExistence(long Id, Guid CorrelationId)
    : CheckEntityExistence(Id: Id, EntityType: EntityType.HrimUser, CorrelationId: CorrelationId);

/// <summary> Checks the existence of a user event type in the storage </summary>
public record CheckEventTypeExistence(long Id, Guid CorrelationId)
    : CheckEntityExistence(Id: Id, EntityType: EntityType.EventType, CorrelationId: CorrelationId);
