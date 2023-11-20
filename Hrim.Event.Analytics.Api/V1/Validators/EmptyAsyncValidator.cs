using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities;

namespace Hrim.Event.Analytics.Api.V1.Validators;

/// <summary> An empty validator does nothing </summary>
/// <typeparam name="TEntity"></typeparam>
public class EmptyAsyncValidator<TEntity, TKey>: AbstractValidator<TEntity>
where TKey: struct
where TEntity: HrimEntity<TKey>
{
}