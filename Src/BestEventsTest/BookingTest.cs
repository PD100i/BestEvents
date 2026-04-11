using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BestEvents;

namespace BestEventsTest
{
    public class BookingTest
    {
        [Fact]
        public void BookingConstructor_CorrectId_InitializeProperties()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            // Act
            var booking = new Booking(eventId);

            // Assert
            Assert.NotEqual(Guid.Empty, booking.Id);
            Assert.Equal(eventId, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
            Assert.True((DateTime.UtcNow - booking.CreatedAt).TotalSeconds < 1);
            Assert.Null(booking.ProcessedAt);
        }

        [Fact]
        public void BookingConstructor_EmptyEventId_ThrowBookingWrongParameterException()
        {
            // Arrange
            var eventId = Guid.Empty;

            // Act & Assert
            Assert.Throws<BestEvents.Exceptions.BookingWrongParameterException>(() => new Booking(eventId));
        }

        [Fact]
        public void Confirm_SetStatusAndProcessedAt()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);

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
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);

            // Act & Assert
            booking.Confirm();
            var firstProcessedAt = booking.ProcessedAt;            
            Assert.Throws<BestEvents.Exceptions.ServiceInvalidOperationException>(() => booking.Confirm());
            Assert.Equal(firstProcessedAt, booking.ProcessedAt ); 
        }

        [Fact]
        public void Reject_SetStatusAndProcessedAt()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);

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
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);

            // Act & Assert
            booking.Reject();
            var firstProcessedAt = booking.ProcessedAt;
            Assert.Throws<BestEvents.Exceptions.ServiceInvalidOperationException>(() => booking.Reject());
            Assert.Equal(firstProcessedAt, booking.ProcessedAt);
        }
    }
}
