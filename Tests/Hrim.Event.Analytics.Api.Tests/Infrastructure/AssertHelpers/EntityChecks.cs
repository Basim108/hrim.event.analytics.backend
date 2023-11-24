using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;

public static class EntityChecks
{
    /// <summary> Assert entity properties after successful entity creation </summary>
    public static void CheckEntitySuccessfulCreation<TEntity>(this TEntity? entity,
                                                              DateTime?     beforeSend = null,
                                                              long?         operatorId = null)
        where TEntity : HrimEntity<long> {
        entity.Should().NotBeNull();
        entity!.Id.Should().NotBe(0);
        if (operatorId.HasValue && entity is IHasOwner ownerResult)
            ownerResult.CreatedById.Should().Be(expected: operatorId.Value);
        if (beforeSend.HasValue)
            entity.CreatedAt.Should().BeAfter(expected: beforeSend.Value);
        entity.UpdatedAt.Should().Be(entity.CreatedAt);
        entity.IsDeleted.Should().BeNull();
        entity.ConcurrentToken.Should().Be(expected: 1);
    }

    /// <summary> Assert entity properties after successful entity creation </summary>
    public static void CheckEntitySuccessfulUpdate<TEntity>(this TEntity? entity,
                                                            DateTime      beforeSend,
                                                            long?         operatorId,
                                                            TEntity       forUpdate)
        where TEntity : HrimEntity<long> {
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(expected: forUpdate.Id);
        if (operatorId.HasValue && entity is IHasOwner ownerResult)
            ownerResult.CreatedById.Should().Be(expected: operatorId.Value);
        entity.CreatedAt.Should().Be(expected: forUpdate.CreatedAt);
        entity.UpdatedAt.Should().BeAfter(expected: beforeSend);
        entity.IsDeleted.Should().BeNull();
        entity.ConcurrentToken.Should().Be(expected: 2);
    }
}