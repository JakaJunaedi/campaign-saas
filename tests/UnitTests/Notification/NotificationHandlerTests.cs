namespace CampaignSaaS.UnitTests.Notification;

using CampaignSaaS.Modules.Notification.Application.Abstractions;
using CampaignSaaS.Modules.Notification.Application.Commands.CreateNotification;
using CampaignSaaS.Modules.Notification.Application.Commands.MarkAllNotificationsAsRead;
using CampaignSaaS.Modules.Notification.Application.Commands.MarkNotificationAsRead;
using CampaignSaaS.Modules.Notification.Application.Queries.GetNotifications;
using CampaignSaaS.Modules.Notification.Application.Queries.GetUnreadNotificationCount;
using CampaignSaaS.Modules.Notification.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class NotificationHandlerTests
{
    private readonly INotificationRepository _repository = Substitute.For<INotificationRepository>();
    private readonly INotificationUnitOfWork _unitOfWork = Substitute.For<INotificationUnitOfWork>();

    [Fact]
    public async Task CreateNotificationCommandHandler_ShouldCreateAndSaveNotification()
    {
        // Arrange
        var handler = new CreateNotificationCommandHandler(_repository, _unitOfWork);
        var command = new CreateNotificationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            "Message",
            "/link");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Title.Should().Be("Title");
        result.Value.Message.Should().Be("Message");
        result.Value.LinkUrl.Should().Be("/link");
        result.Value.IsRead.Should().BeFalse();
        await _repository.Received(1).AddAsync(Arg.Any<InAppNotification>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkNotificationAsReadCommandHandler_WhenFound_ShouldMarkAsRead()
    {
        // Arrange
        var notification = InAppNotification.Create(Guid.NewGuid(), Guid.NewGuid(), "Title", "Message");
        _repository.GetByIdAsync(notification.Id, Arg.Any<CancellationToken>())
            .Returns(notification);

        var handler = new MarkNotificationAsReadCommandHandler(_repository, _unitOfWork);
        var command = new MarkNotificationAsReadCommand(notification.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        notification.IsRead.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkNotificationAsReadCommandHandler_WhenNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var notifId = Guid.NewGuid();
        _repository.GetByIdAsync(notifId, Arg.Any<CancellationToken>())
            .Returns((InAppNotification?)null);

        var handler = new MarkNotificationAsReadCommandHandler(_repository, _unitOfWork);
        var command = new MarkNotificationAsReadCommand(notifId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Notification.NotFound");
    }

    [Fact]
    public async Task MarkAllNotificationsAsReadCommandHandler_ShouldCallRepositoryAndSave()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var handler = new MarkAllNotificationsAsReadCommandHandler(_repository, _unitOfWork);
        var command = new MarkAllNotificationsAsReadCommand(userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        await _repository.Received(1).MarkAllAsReadAsync(userId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetNotificationsQueryHandler_ShouldReturnDtoList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notifications = new List<InAppNotification>
        {
            InAppNotification.Create(Guid.NewGuid(), userId, "Title 1", "Msg 1"),
            InAppNotification.Create(Guid.NewGuid(), userId, "Title 2", "Msg 2")
        };

        _repository.GetByUserIdAsync(userId, null, 50, Arg.Any<CancellationToken>())
            .Returns(notifications);

        var handler = new GetNotificationsQueryHandler(_repository);
        var query = new GetNotificationsQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value[0].Title.Should().Be("Title 1");
        result.Value[1].Title.Should().Be("Title 2");
    }

    [Fact]
    public async Task GetUnreadNotificationCountQueryHandler_ShouldReturnCount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repository.GetUnreadCountAsync(userId, Arg.Any<CancellationToken>())
            .Returns(5);

        var handler = new GetUnreadNotificationCountQueryHandler(_repository);
        var query = new GetUnreadNotificationCountQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(5);
    }
}
