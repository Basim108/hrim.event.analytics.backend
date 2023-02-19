using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;

public static class EntityChecks
{
    /// <summary> Assert entity properties after successful entity creation </summary>
    public static void CheckEntitySuccessfulCreation<TEntity>(this TEntity? entity, DateTime? beforeSend = null,
        Guid? operatorId = null)
        where TEntity : HrimEntity
    {
        entity.Should().NotBeNull();
        entity!.Id.Should().NotBeEmpty();
        if (operatorId.HasValue && entity is IHasOwner ownerResult)
            ownerResult.CreatedById.Should().Be(operatorId.Value);
        if (beforeSend.HasValue)
            entity.CreatedAt.Should().BeAfter(beforeSend.Value);
        entity.UpdatedAt.Should().BeNull();
        entity.IsDeleted.Should().BeNull();
        entity.ConcurrentToken.Should().Be(1);
    }

    /// <summary> Assert entity properties after successful entity creation </summary>
    public static void CheckEntitySuccessfulUpdate<TEntity>(this TEntity? entity, DateTime beforeSend, Guid? operatorId,
        TEntity forUpdate)
        where TEntity : HrimEntity
    {
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(forUpdate.Id);
        if (operatorId.HasValue && entity is IHasOwner ownerResult)
            ownerResult.CreatedById.Should().Be(operatorId.Value);
        entity.CreatedAt.Should().Be(forUpdate.CreatedAt);
        entity.UpdatedAt.Should().BeAfter(beforeSend);
        entity.IsDeleted.Should().BeNull();
        entity.ConcurrentToken.Should().Be(2);
    }
}