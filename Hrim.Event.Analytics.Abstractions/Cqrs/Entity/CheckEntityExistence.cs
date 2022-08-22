using Hrim.Event.Analytics.Abstractions.Enums;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Entity;

/// <summary> Checks the existence of entity in the storage </summary>
/// <param name="Id">Id of the entity that has to be checked</param>
/// <param name="EntityType">type of entity</param>
/// <param name="CorrelationId"><see cref="BaseRequest"/></param>
public record CheckEntityExistence(Guid Id, EntityType EntityType, Guid CorrelationId)
    : BaseRequest(CorrelationId), IRequest<CqrsVoidResult>;
    
/// <summary> Checks the existence of a user in the storage </summary>
public record CheckUserExistence(Guid Id, Guid CorrelationId)
    : CheckEntityExistence(Id, EntityType.HrimUser, CorrelationId);
    
/// <summary> Checks the existence of a user event type in the storage </summary>
public record CheckEventTypeExistence(Guid Id, Guid CorrelationId)
    : CheckEntityExistence(Id, EntityType.EventType, CorrelationId);