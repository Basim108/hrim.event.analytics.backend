// Copyright © 2021 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;

namespace Hrim.Event.Analytics.Api.Tests.Exceptions;

public class UnsupportedEntityExceptionTests
{
    [Fact]
    public void Should_Set_Message_and_EntityType_Property() {
        var entityType = typeof(String);
        var ex   = new UnsupportedEntityException(entityType);
        ex.Message.Should().Be($"Unsupported entity of type: {entityType.FullName}");
        ex.EntityType.Should().Be(entityType);
    }
}