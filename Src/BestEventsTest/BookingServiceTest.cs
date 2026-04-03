using BestEvents;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsTest
{
    public class BookingServiceFixture
    {
        public Mock<IEventRepository> MockEventRepository { get; set; }

        public Mock<IBookingRepository> MockBookingRepository { get; set; }

        public BookingService BookingService { get; set; }

        public BookingServiceFixture()
        {
            MockEventRepository = new Mock<IEventRepository>();
            MockBookingRepository = new Mock<IBookingRepository>();
            BookingService = new BookingService(MockEventRepository.Object, MockBookingRepository.Object);
        }
    }
    public class BookingServiceTest
    {
        [Fact]
        public async Task CreateBookingAsync_EventExists_ReturnBookingResultDto()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);
            fixture.MockBookingRepository.Setup(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None);

            // Assert
            fixture.MockEventRepository.Verify(repo => repo.GetEvent(eventId), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(booking.Id.ToString(), result.Id);
            Assert.Equal(booking.EventId.ToString(), result.EventId);
            Assert.Equal(booking.Status.ToString(), result.Status);
            Assert.Equal(booking.CreatedAt, result.CreatedAt);
            Assert.Equal(eventId.ToString(), result.EventId);
            Assert.Equal(BookingStatus.Pending.ToString(), result.Status);
            Assert.Null(result.ProcessedAt);
        }

        [Fact]
        public async Task CreateBookingAsync_MultipleRequestes_CreateSomeBooking()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            
            fixture.MockBookingRepository.Setup(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), CancellationToken.None))
               .ReturnsAsync((Booking booking, CancellationToken ct) => booking);

            List<Task<BookingResultDto>> t = [
                fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None),
                fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None),
                fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None)];

            // Act
            var result = await Task.WhenAll(t);

            // Assert
            Assert.Equal(eventId.ToString(), result[0].EventId);
            Assert.Null(result[0].ProcessedAt);
            Assert.Equal(BookingStatus.Pending.ToString(), result[0].Status);

            Assert.Equal(eventId.ToString(), result[1].EventId);
            Assert.Null(result[1].ProcessedAt);
            Assert.Equal(BookingStatus.Pending.ToString(), result[1].Status);

            Assert.Equal(eventId.ToString(), result[2].EventId);
            Assert.Null(result[2].ProcessedAt);
            Assert.Equal(BookingStatus.Pending.ToString(), result[2].Status);

            Assert.NotEqual(result[0].Id, result[1].Id);
            Assert.NotEqual(result[1].Id, result[2].Id);

        }

        [Fact]
        public async Task CreateBookingAsync_EventDoesNotExist_ShouldThrowDataNotFoundException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            fixture.MockEventRepository.Setup(repo => repo.GetEvent(eventId)).Throws(new BestEvents.Exceptions.EventNotFoundException("Событие не найдено"));

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => fixture.BookingService.CreateBookingAsync(eventId.ToString(), CancellationToken.None));
            fixture.MockEventRepository.Verify(repo => repo.GetEvent(eventId), Times.Once);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_WrongFormatEventId_ShouldThrowBookingWrongParameterException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var invalidEventId = "123";

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingWrongParameterException>(() => fixture.BookingService.CreateBookingAsync(invalidEventId, CancellationToken.None));
            fixture.MockEventRepository.Verify(repo => repo.GetEvent(It.IsAny<Guid>()), Times.Never);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_Cancel_Cancelled()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => fixture.BookingService.CreateBookingAsync(eventId.ToString(), cts.Token));
            fixture.MockEventRepository.Verify(repo => repo.GetEvent(It.IsAny<Guid>()), Times.Never);
            fixture.MockBookingRepository.Verify(repo => repo.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task GetBookingByIdAsync_BookingExists_ShouldReturnBookingResultDto()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);
            fixture.MockBookingRepository.Setup(repo => repo.GetBookingAsync(booking.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await fixture.BookingService.GetBookingByIdAsync(booking.Id.ToString(), CancellationToken.None);

            // Assert
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(booking.Id, It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(booking.Id.ToString(), result.Id);
            Assert.Equal(booking.EventId.ToString(), result.EventId);
            Assert.Equal(booking.Status.ToString(), result.Status);
            Assert.Equal(booking.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task GetBookingByIdAsync_BookingDoesNotExist_ShouldThrowDataNotFoundException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var bookingId = Guid.NewGuid();
            fixture.MockBookingRepository.Setup(repo => repo.GetBookingAsync(bookingId, It.IsAny<CancellationToken>()))
                .Throws(new BestEvents.Exceptions.EventNotFoundException("Бронирование не найдено"));

            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.EventNotFoundException>(() => fixture.BookingService.GetBookingByIdAsync(bookingId.ToString(), CancellationToken.None));
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBookingByIdAsync_WrongFormatBookingId_ShouldThrowBookingWrongParameterException()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            string invalidBookingId = "123";
            // Act & Assert
            await Assert.ThrowsAsync<BestEvents.Exceptions.BookingWrongParameterException>(() => fixture.BookingService.GetBookingByIdAsync(invalidBookingId, CancellationToken.None));
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetBookingByIdAsync_Cancel_Cancelled()
        {
            // Arrange
            var fixture = new BookingServiceFixture();
            var bookingId = Guid.NewGuid();
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => fixture.BookingService.GetBookingByIdAsync(bookingId.ToString(), cts.Token));
            fixture.MockBookingRepository.Verify(repo => repo.GetBookingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
