using BestEvents.Domain;
using BestEvents.Domain.Exceptions;

namespace BestEventsTest
{
    public class BookingTest
    {
        private Event CreateEvent()
        {
            return Event.CreateInstanceEvent(Guid.NewGuid(), "TestEvent", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5), "Description", 100, 50);
        }

        [Fact]
        public void BookingConstructor_CorrectId_InitializeProperties()
        {
            // Arrange
            var _event = CreateEvent();
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");

            // Act
            var booking = new Booking(bookingId, _event, user);

            // Assert
            Assert.NotEqual(Guid.Empty, booking.Id);
            Assert.Equal(_event.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
            Assert.True((DateTime.UtcNow - booking.CreatedAt).TotalSeconds < 1);
            Assert.Null(booking.ProcessedAt);
        }



        [Fact]
        public void Confirm_SetStatusAndProcessedAt()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var booking = new Booking(bookingId, CreateEvent(), user);

            // Act
            booking.Confirm();

            // Assert
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
            Assert.NotNull(booking.ProcessedAt);
        }

        [Fact]
        public void Confirm_MultipleTimes_ShouldSetStatusAndProcessedAtOnlyOnce()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var booking = new Booking(bookingId, CreateEvent(), user);

            // Act & Assert
            booking.Confirm();
            var firstProcessedAt = booking.ProcessedAt;
            Assert.Throws<BookingDoubleProcessingException>(() => booking.Confirm());
            Assert.Equal(firstProcessedAt, booking.ProcessedAt);
        }

        [Fact]
        public void Reject_SetStatusAndProcessedAt()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var booking = new Booking(bookingId, CreateEvent(), user);

            // Act
            booking.Reject();

            // Assert
            Assert.Equal(BookingStatus.Rejected, booking.Status);
            Assert.NotNull(booking.ProcessedAt);
        }

        [Fact]
        public void Reject_MultipleTimes_ShouldSetStatusAndProcessedAtOnlyOnce()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var booking = new Booking(bookingId, CreateEvent(), user);

            // Act & Assert
            booking.Reject();
            var firstProcessedAt = booking.ProcessedAt;
            Assert.Throws<BookingDoubleProcessingException>(() => booking.Reject());
            Assert.Equal(firstProcessedAt, booking.ProcessedAt);
        }

        [Fact]
        public void CancelBookingWithUserStatus_ChangeStatusToCancelled()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var booking = new Booking(bookingId, CreateEvent(), user);
            // Act
            booking.Cancel(user);
            // Assert
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public void CancelBookingWithAdminStatus_ChangeStatusToCancelled()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var admin = User.CreateUser(Guid.NewGuid(), "TestAdmin", "hashedAdminPassword", "Admin");
            var booking = new Booking(bookingId, CreateEvent(), user);
            // Act
            booking.Cancel(user);
            // Assert
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public void CancelBookingWithOtherUser_ShouldThrowException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var user = User.CreateUser(Guid.NewGuid(), "TestUser", "hashedpassword", "User");
            var otherUser = User.CreateUser(Guid.NewGuid(), "OtherUser", "hashedpassword", "User");
            var booking = new Booking(bookingId, CreateEvent(), user);
            // Act & Assert
            Assert.Throws<NoRightOfCancelBookingException>(() => booking.Cancel(otherUser));
        }
    }
}
