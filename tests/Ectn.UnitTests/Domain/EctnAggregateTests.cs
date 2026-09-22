using System;
using Xunit;
using FluentAssertions;
using Ectn.Domain.Entities;
using Ectn.Domain.Enums;

namespace Ectn.UnitTests.Domain;

public class EctnAggregateTests
{
    [Fact]
    public void Submit_Should_ChangeStatusToSubmitted_When_StatusIsDraft()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var ectn = new Ectn.Domain.Entities.Ectn(tenantId, "APP-REF-123", EctnType.Standard, actorId);

        // Act
        ectn.Submit(actorId);

        // Assert
        ectn.Status.Should().Be(EctnStatus.Submitted);
        ectn.SubmittedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ectn.StatusHistories.Should().ContainSingle(h => h.Status == EctnStatus.Submitted);
    }

    [Fact]
    public void Submit_Should_ThrowException_When_StatusIsNotDraft()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var ectn = new Ectn.Domain.Entities.Ectn(tenantId, "APP-REF-123", EctnType.Standard, actorId);
        ectn.Submit(actorId); // Status is now Submitted

        // Act & Assert
        Action action = () => ectn.Submit(actorId);
        action.Should().Throw<InvalidOperationException>()
              .WithMessage("Only draft ECTNs can be submitted.");
    }
}
