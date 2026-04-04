using BestEvents;
using BestEvents.Exceptions;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEventsTest
{
    public class BookingProcessorFixture
    {
        public int OneCycleTime { get; set; } = 2200;
        public BookingProcesser BookingProcessor {  get; set; }
        public Mock<IEventRepository> MockEventRepository { get; set; }
        public Mock<IBookingRepository> MockBookingRepository {  get; set; }
       
        
        public BookingProcessorFixture() 
        {
            MockEventRepository = new Mock<IEventRepository>();
            MockBookingRepository = new Mock<IBookingRepository>();
            var mockLogger = new Mock<ILogger<BookingProcesser>>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            var mockScope = new Mock<IServiceScope>();

            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);
            mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);
            mockServiceProvider.Setup(s => s.GetService(typeof(IBookingRepository))).Returns(MockBookingRepository.Object);
            mockServiceProvider.Setup(s => s.GetService(typeof(IEventRepository))).Returns(MockEventRepository.Object);

            BookingProcessor = new BookingProcesser(mockScopeFactory.Object, mockLogger.Object);
        }
    }

    public class BookingProcesserTest
    {
        [Fact]
        public async Task ExecuteAsync_IsFoundOnePendingBookingAndEventExist_ConfirmBooking()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);
            List<Booking> bookings = [booking];
            var _event = new Event(eventId, "AvailableEvent", DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "");
            var fixture = new BookingProcessorFixture();
            fixture.MockBookingRepository.Setup(r => r.GetPendingBookingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings.Where(i => i.Status == BookingStatus.Pending).ToList());

            fixture.MockEventRepository.Setup(r => r.GetEvent(eventId)).Returns(_event);
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // Act
            await fixture.BookingProcessor.StartAsync(ct);
            await Task.Delay(fixture.OneCycleTime, ct);
            await fixture.BookingProcessor.StopAsync(ct);

            // Assert
            fixture.MockBookingRepository.Verify(r => r.ReplaceBookingAsync(It.Is<Booking>(b => b.Status == BookingStatus.Confirmed), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_IsFoundOnePendingBookingButEventIsNotFound_RejectBooking()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);
            List<Booking> bookings = [booking];
            var fixture = new BookingProcessorFixture();
            fixture.MockBookingRepository.Setup(r => r.GetPendingBookingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings.Where(i => i.Status == BookingStatus.Pending).ToList());

            fixture.MockEventRepository.Setup(r => r.GetEvent(eventId)).Throws(new EventNotFoundException("Событие не найдено"));
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // Act
            await fixture.BookingProcessor.StartAsync(ct);
            await Task.Delay(fixture.OneCycleTime, ct);
            await fixture.BookingProcessor.StopAsync(ct);

            // Assert
            fixture.MockBookingRepository.Verify(r => r.ReplaceBookingAsync(It.Is<Booking>(b => b.Status == BookingStatus.Rejected), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_IsFoundOnePendingBookingButEventPassed_ConfirmBooking()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId);
            List<Booking> bookings = [booking];
            var _event = new Event(eventId, "EventPassed", DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1), "");
            var fixture = new BookingProcessorFixture();
            fixture.MockBookingRepository.Setup(r => r.GetPendingBookingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings.Where(i => i.Status == BookingStatus.Pending).ToList());

            fixture.MockEventRepository.Setup(r => r.GetEvent(eventId)).Returns(_event);
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            // Act
            await fixture.BookingProcessor.StartAsync(ct);
            await Task.Delay(fixture.OneCycleTime, ct);
            await fixture.BookingProcessor.StopAsync(ct);

            // Assert
            fixture.MockBookingRepository.Verify(r => r.ReplaceBookingAsync(It.Is<Booking>(b => b.Status == BookingStatus.Rejected), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
