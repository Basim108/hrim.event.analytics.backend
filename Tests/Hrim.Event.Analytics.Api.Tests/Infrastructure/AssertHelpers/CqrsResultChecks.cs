using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;

/// <summary>
///     CqrsResult{T} shared assertions
/// </summary>
public static class CqrsResultChecks
{
    /// <summary> Checks common for creation entity properties </summary>
    public static void CheckSuccessfullyCreatedEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult,
                                                               Guid?                     operatorId,
                                                               DateTime                  beforeSend)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Created);

        cqrsResult.Result.CheckEntitySuccessfulCreation(beforeSend: beforeSend, operatorId: operatorId);
    }

    /// <summary>
    ///     Checks entity creation result when the entity with same unique properties exists and is in soft-deleted state
    /// </summary>
    public static void CheckUpdateOrCreationOfSoftDeletedEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.EntityIsDeleted);
        cqrsResult.Info.Should().BeNull();

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.IsDeleted.Should().BeTrue();
    }

    /// <summary>
    ///     Checks entity creation result when the entity with same unique properties exists
    /// </summary>
    public static void CheckCreationOfSameEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Conflict);
    }

    /// <summary> Checks common for update entity properties </summary>
    public static void CheckSuccessfullyUpdatedEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult,
                                                               Guid?                     operatorId,
                                                               TEntity                   forUpdate,
                                                               DateTime                  beforeSend)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Ok);

        cqrsResult.Result.CheckEntitySuccessfulUpdate(beforeSend: beforeSend, operatorId: operatorId, forUpdate: forUpdate);
    }

    /// <summary> Checks common for update entity properties </summary>
    public static void CheckConcurrentConflictUpdate<TEntity>(this CqrsResult<TEntity?> cqrsResult, TEntity forUpdate)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(expected: CqrsResultCode.Conflict);

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(expected: forUpdate.Id);
        cqrsResult.Result.ConcurrentToken.Should().Be(expected: 1);
        cqrsResult.Result.UpdatedAt.Should().BeNull();
    }
}