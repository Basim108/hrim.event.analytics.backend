using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;

/// <summary>
/// CqrsResult{T} shared assertions
/// </summary>
public static class CqrsResultChecks {
    /// <summary> Checks common for creation entity properties </summary>
    public static void CheckSuccessfullyCreatedEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult,
                                                               Guid?                     operatorId,
                                                               DateTime                  beforeSend)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Created);

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().NotBeEmpty();
        if (operatorId.HasValue && cqrsResult.Result is IHasOwner ownerResult)
            ownerResult.CreatedById.Should().Be(operatorId.Value);
        cqrsResult.Result.CreatedAt.Should().BeAfter(beforeSend);
        cqrsResult.Result.UpdatedAt.Should().BeNull();
        cqrsResult.Result.IsDeleted.Should().BeNull();
        cqrsResult.Result.ConcurrentToken.Should().Be(1);
    }

    /// <summary>
    /// Checks entity creation result when the entity with same unique properties exists and is in soft-deleted state
    /// </summary>
    public static void CheckUpdateOrCreationOfSoftDeletedEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.EntityIsDeleted);
        cqrsResult.Info.Should().BeNull();

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.IsDeleted.Should().BeTrue();
    }

    /// <summary>
    /// Checks entity creation result when the entity with same unique properties exists 
    /// </summary>
    public static void CheckCreationOfSameEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Conflict);
    }

    /// <summary> Checks common for update entity properties </summary>
    public static void CheckSuccessfullyUpdatedEntity<TEntity>(this CqrsResult<TEntity?> cqrsResult,
                                                               Guid?                     operatorId,
                                                               TEntity                   forUpdate,
                                                               DateTime                  beforeSend)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Ok);

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(forUpdate.Id);
        if (operatorId.HasValue && cqrsResult.Result is IHasOwner ownerResult)
            ownerResult.CreatedById.Should().Be(operatorId.Value);
        cqrsResult.Result.CreatedAt.Should().Be(forUpdate.CreatedAt);
        cqrsResult.Result.UpdatedAt.Should().BeAfter(beforeSend);
        cqrsResult.Result.IsDeleted.Should().BeNull();
        cqrsResult.Result.ConcurrentToken.Should().Be(2);
    }

    /// <summary> Checks common for update entity properties </summary>
    public static void CheckConcurrentConflictUpdate<TEntity>(this CqrsResult<TEntity?> cqrsResult, TEntity forUpdate)
        where TEntity : HrimEntity {
        cqrsResult.Should().NotBeNull();
        cqrsResult.StatusCode.Should().Be(CqrsResultCode.Conflict);

        cqrsResult.Result.Should().NotBeNull();
        cqrsResult.Result!.Id.Should().Be(forUpdate.Id);
        cqrsResult.Result.ConcurrentToken.Should().Be(1);
        cqrsResult.Result.UpdatedAt.Should().BeNull();
    }
}