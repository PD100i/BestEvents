using BestEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsTest
{
    public class BookingTest
    {
        private Event CreateEvent()
        {
            return Event.CreateInstanceEvent(Guid.NewGuid(), "Test Event", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5), "Description", 100, 50);
        }

        [Fact]
        public void BookingConstructor_CorrectId_InitializeProperties()
        {
            // Arrange
            var _event = CreateEvent();
            var bookingId = Guid.NewGuid();

            // Act
            var booking = new Booking(bookingId, _event);

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
            var booking = new Booking(bookingId, CreateEvent());

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
            var booking = new Booking(bookingId, CreateEvent());

            // Act & Assert
            booking.Confirm();
            var firstProcessedAt = booking.ProcessedAt;            
            Assert.Throws<BestEvents.Exceptions.BookingDoubleProcessingException>(() => booking.Confirm());
            Assert.Equal(firstProcessedAt, booking.ProcessedAt ); 
        }

        [Fact]
        public void Reject_SetStatusAndProcessedAt()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var booking = new Booking(bookingId, CreateEvent());

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
            var booking = new Booking(bookingId, CreateEvent());

            // Act & Assert
            booking.Reject();
            var firstProcessedAt = booking.ProcessedAt;
            Assert.Throws<BestEvents.Exceptions.BookingDoubleProcessingException>(() => booking.Reject());
            Assert.Equal(firstProcessedAt, booking.ProcessedAt);
        }
    }
}
