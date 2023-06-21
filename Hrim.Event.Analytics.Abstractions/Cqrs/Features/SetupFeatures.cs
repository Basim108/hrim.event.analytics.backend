using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Features;

/// <summary> Setup features from env variables in the storage </summary>
public record SetupFeatures(): IRequest<Unit>;