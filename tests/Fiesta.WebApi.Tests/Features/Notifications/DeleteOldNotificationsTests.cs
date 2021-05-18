using System;
using System.Threading.Tasks;
using Fiesta.Application.Common.Interfaces;
using Fiesta.Application.Features.Notifications;
using Fiesta.Application.Models.Notifications;
using Fiesta.Domain.Entities.Notifications;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TestBase;
using TestBase.Assets;
using TestBase.Helpers;
using Xunit;

namespace Fiesta.WebApi.Tests.Features.Notifications
{
    [Collection(nameof(TestCollection))]
    public class DeleteOldNotificationsTests : DbTestBase
    {
        private IDateTimeProvider _dateTime;

        public DeleteOldNotificationsTests()
        {
            _dateTime = Substitute.For<IDateTimeProvider>();
            _dateTime.UtcNow.Returns(new DateTime(2021, 1, 1));
        }

        [Fact]
        public async Task GivenSomeNotificationsStore_WhenDeletingOldOnes_OnlyOldAreDeleted()
        {
            var (_, user) = ArrangeDb.SeedBasicUser();
            var notification1 = new Notification(user, new EventAttendeeRemoved());
            var notification2 = new Notification(user, new EventAttendeeRemoved());
            var notification3 = new Notification(user, new EventAttendeeRemoved());

            notification1.Set("CreatedAt", _dateTime.UtcNow.Subtract(TimeSpan.FromDays(33)));
            notification2.Set("CreatedAt", _dateTime.UtcNow.Subtract(TimeSpan.FromDays(44)));
            notification3.Set("CreatedAt", _dateTime.UtcNow.Subtract(TimeSpan.FromDays(1)));

            ArrangeDb.AddRange(new[] { notification1, notification2, notification3 });
            await ArrangeDb.SaveChangesAsync();

            var command = new DeleteOldNotifications.Command
            {
                BatchSize = 1,
                DeleteAfter = TimeSpan.FromDays(20)
            };

            var handler = new DeleteOldNotifications.Handler(ActDb, _dateTime);
            await handler.Handle(command, default);

            var notificationDb = await AssertDb.Notifications.SingleAsync();
            notificationDb.Id.Should().Be(notification3.Id);
        }
    }
}
