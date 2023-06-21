using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Services;
using MediatR;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;

/// <summary> An empty validator does nothing </summary>
/// <typeparam name="TEntity"></typeparam>
public class EmptyAsyncValidator<TEntity>: AbstractValidator<TEntity>
where TEntity: HrimEntity
{
}