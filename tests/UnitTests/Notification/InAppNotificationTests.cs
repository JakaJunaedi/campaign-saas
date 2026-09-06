namespace CampaignSaaS.UnitTests.Notification;

using CampaignSaaS.Modules.Notification.Domain.Entities;
using CampaignSaaS.Modules.Notification.Domain.Events;
using FluentAssertions;
using Xunit;

public class InAppNotificationTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateNotificationAndEmitDomainEvent()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "This is a test notification message.";
        var linkUrl = "/campaigns/123";

        // Act
        var notification = InAppNotification.Create(orgId, userId, title, message, linkUrl);

        // Assert
        notification.Should().NotBeNull();
        notification.Id.Should().NotBeEmpty();
        notification.OrganizationId.Should().Be(orgId);
        notification.UserId.Should().Be(userId);
        notification.Title.Should().Be(title);
        notification.Message.Should().Be(message);
        notification.LinkUrl.Should().Be(linkUrl);
        notification.IsRead.Should().BeFalse();
        notification.ReadAt.Should().BeNull();
        notification.DomainEvents.Should().ContainSingle(e => e is NotificationCreatedDomainEvent);
    }

    [Theory]
    [InlineData("", "Valid message")]
    [InlineData("   ", "Valid message")]
    [InlineData("Valid title", "")]
    [InlineData("Valid title", "   ")]
    public void Create_WithInvalidParameters_ShouldThrowArgumentException(string title, string message)
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var act = () => InAppNotification.Create(orgId, userId, title, message);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkAsRead_WhenUnread_ShouldSetIsReadAndEmitDomainEvent()
    {
        // Arrange
        var notification = InAppNotification.Create(Guid.NewGuid(), Guid.NewGuid(), "Title", "Message");

        // Act
        notification.MarkAsRead();

        // Assert
        notification.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        notification.DomainEvents.Should().Contain(e => e is NotificationReadDomainEvent);
    }

    [Fact]
    public void MarkAsRead_WhenAlreadyRead_ShouldNotEmitDuplicateDomainEvent()
    {
        // Arrange
        var notification = InAppNotification.Create(Guid.NewGuid(), Guid.NewGuid(), "Title", "Message");
        notification.MarkAsRead();
        var initialReadAt = notification.ReadAt;

        // Act
        notification.MarkAsRead();

        // Assert
        notification.ReadAt.Should().Be(initialReadAt);
        notification.DomainEvents.OfType<NotificationReadDomainEvent>().Should().HaveCount(1);
    }
}
