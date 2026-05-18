using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BestEvents;

namespace BestEventsTest
{
    public class EntityMapperTest
    {
        [Fact]
        public void MapEventToEventEntity_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var _event = Event.CreateInstanceEvent(Guid.NewGuid(), "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), "Description", 100, 50);

            // Act
            var eventEntity = mapper.MapEventToEntity(_event);

            // Assert
            Assert.Equal(_event.Id, eventEntity.Id);
            Assert.Equal(_event.Title, eventEntity.Title);
            Assert.Equal(_event.StartAt, eventEntity.StartAt);
            Assert.Equal(_event.EndAt, eventEntity.EndAt);
            Assert.Equal(_event.Description, eventEntity.Description);
            Assert.Equal(_event.TotalSeats, eventEntity.TotalSeats);
            Assert.Equal(_event.AvailableSeats, eventEntity.AvailableSeats);
        }

        [Fact]
        public void MapEventEntityToEvent_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var eventEntity = new EventEntity(){
                Id = Guid.NewGuid(), 
                Title = "Test Event", 
                StartAt = DateTime.UtcNow, 
                EndAt = DateTime.UtcNow.AddHours(2), 
                Description = "Description", 
                TotalSeats = 100, 
                AvailableSeats = 50 };

            // Act
            var _event = mapper.MapEntityToEvent(eventEntity);

            // Assert
            Assert.Equal(eventEntity.Id, _event.Id);
            Assert.Equal(eventEntity.Title, _event.Title);
            Assert.Equal(eventEntity.StartAt, _event.StartAt);
            Assert.Equal(eventEntity.EndAt, _event.EndAt);
            Assert.Equal(eventEntity.Description, _event.Description);
            Assert.Equal(eventEntity.TotalSeats, _event.TotalSeats);
            Assert.Equal(eventEntity.AvailableSeats, _event.AvailableSeats);
        }

        [Fact]
        public void UpdateEventEntity_ShouldUpdateCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var _event = Event.CreateInstanceEvent(Guid.NewGuid(), "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), "Description", 100, 50);
            var eventEntity = new EventEntity()
            {
                Id = _event.Id,
                Title = "Test Event 2",
                StartAt = DateTime.UtcNow.AddHours(1),
                EndAt = DateTime.UtcNow.AddHours(2),
                Description = "Description2",
                TotalSeats = 101,
                AvailableSeats = 52
            };

            // Act
            mapper.UpdateEventEntity(_event, eventEntity);

            // Assert
            Assert.Equal(_event.Id, eventEntity.Id);
            Assert.Equal(_event.Title, eventEntity.Title);
            Assert.Equal(_event.StartAt, eventEntity.StartAt);
            Assert.Equal(_event.EndAt, eventEntity.EndAt);
            Assert.Equal(_event.Description, eventEntity.Description);
            Assert.Equal(_event.TotalSeats, eventEntity.TotalSeats);
            Assert.Equal(_event.AvailableSeats, eventEntity.AvailableSeats);
        }

        [Fact]
        public void UpdateEvent_ShouldUpdateCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var _event = Event.CreateInstanceEvent(Guid.NewGuid(), "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), "Description", 100, 50);
            var eventEntity = new EventEntity()
            {
                Id = Guid.NewGuid(),
                Title = "Test Event 2",
                StartAt = DateTime.UtcNow.AddHours(1),
                EndAt = DateTime.UtcNow.AddHours(2),
                Description = "Description2",
                TotalSeats = 101,
                AvailableSeats = 52
            };

            // Act
            mapper.UpdateEvent(eventEntity, _event);

            // Assert
            Assert.Equal(_event.Id, eventEntity.Id);
            Assert.Equal(_event.Title, eventEntity.Title);
            Assert.Equal(_event.StartAt, eventEntity.StartAt);
            Assert.Equal(_event.EndAt, eventEntity.EndAt);
            Assert.Equal(_event.Description, eventEntity.Description);
            Assert.Equal(_event.TotalSeats, eventEntity.TotalSeats);
            Assert.Equal(_event.AvailableSeats, eventEntity.AvailableSeats);
        }

        [Fact]
        public void MapBookingToBookingEntity_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var booking = new Booking() {
                Id = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                Event = Event.CreateInstanceEvent(Guid.NewGuid(), "Event 1", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5), "Descripion", 100, 50),
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow.AddMinutes(3) };

            // Act
            var bookingEntity = mapper.MapBookingToEntity(booking);

            // Assert
            Assert.Equal(booking.Id, bookingEntity.Id);
            Assert.Equal(booking.EventId, bookingEntity.EventId);
            Assert.Equal(booking.Status, bookingEntity.Status);
            Assert.Equal(booking.CreatedAt, bookingEntity.CreatedAt);
            Assert.Equal(booking.ProcessedAt, bookingEntity.ProcessedAt);
            Assert.Null(bookingEntity.Event);
            
        }

        [Fact]
        public void MapBookingEntityToBooking_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var eventEntity = new EventEntity()
            {
                Id = Guid.NewGuid(),
                Title = "Event 1",
                StartAt = DateTime.UtcNow.AddHours(-5),
                EndAt = DateTime.UtcNow.AddHours(5),
                Description = "Description",
                TotalSeats = 101,
                AvailableSeats = 52
            };


            var bookingEntity = new BookingEntity()
            {
                Id = Guid.NewGuid(),
                EventId = eventEntity.Id,
                Event = eventEntity,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.Now,
                ProcessedAt = DateTime.UtcNow.AddMinutes(3)
            };



            // Act
            var booking = mapper.MapEntityToBooking(bookingEntity);

            // Assert
            Assert.Equal(bookingEntity.Id, booking.Id);
            Assert.Equal(bookingEntity.EventId, booking.EventId);
            Assert.Equal(bookingEntity.Status, booking.Status);
            Assert.Equal(bookingEntity.CreatedAt, booking.CreatedAt);
            Assert.Equal(bookingEntity.ProcessedAt, booking.ProcessedAt);
            Assert.NotNull(booking.Event);
            Assert.Equal(bookingEntity.Event.Id, booking.Event.Id);
            Assert.Equal(bookingEntity.Event.Title, booking.Event.Title);
            Assert.Equal(bookingEntity.Event.StartAt, booking.Event.StartAt);
            Assert.Equal(bookingEntity.Event.EndAt, booking.Event.EndAt);
            Assert.Equal(bookingEntity.Event.Description, booking.Event.Description);
            Assert.Equal(bookingEntity.Event.TotalSeats, booking.Event.TotalSeats);
            Assert.Equal(bookingEntity.Event.AvailableSeats, booking.Event.AvailableSeats);
        }

        [Fact]
        public void UpdateBookingEntity_ShouldUpdateCorrectly()
        {
            // Arrange
            var mapper = new EntityMapper();
            var booking = new Booking() { 
                Id = Guid.NewGuid(), 
                EventId = Guid.NewGuid(), 
                Status = BookingStatus.Confirmed, 
                CreatedAt = DateTime.UtcNow, 
                ProcessedAt = DateTime.UtcNow.AddMinutes(3) };


            var bookingEntity = new BookingEntity()
            {
                Id = booking.Id,
                EventId = booking.EventId,
                Status = BookingStatus.Pending,
                CreatedAt = booking.CreatedAt,
                ProcessedAt = null
            };
                
            // Act
            mapper.UpdateBookingEntity(booking, bookingEntity);

            // Assert
            Assert.Equal(booking.Id, bookingEntity.Id);
            Assert.Equal(booking.EventId, bookingEntity.EventId);
            Assert.Equal(booking.Status, bookingEntity.Status);
            Assert.Equal(booking.CreatedAt, bookingEntity.CreatedAt);
            Assert.Equal(booking.ProcessedAt, bookingEntity.ProcessedAt);
        }

        

        [Fact]
        public void MapPaginatedResultToPaginatedResultDto()
        {
            // Arrange
            var mapper = new EntityMapper();
            var collection = EventCollection.GetCollection();
            PaginatedResult<EventEntity> paginatedResult = new PaginatedResult<EventEntity>(collection, 1, 100);

            // Act
            var result = mapper.MapPaginatedResultToEntity(paginatedResult);

            // Assert
            Assert.Equal(result.TotalResultsNumber, paginatedResult.TotalResultsNumber);
            Assert.Equal(result.ResultsNumberOnPage, paginatedResult.ResultsNumberOnPage);
            Assert.Equal(result.CurrentPage, paginatedResult.CurrentPage);

            var expectedResultOnPage = paginatedResult.ResultsOnPage.Select(e => new Event() { 
                Id = e.Id, 
                Title = e.Title, 
                Description = e.Description, 
                StartAt = e.StartAt, 
                EndAt = e.EndAt, 
                TotalSeats = e.TotalSeats, 
                AvailableSeats = e.AvailableSeats })
                .ToList();

            Assert.Equal(result.ResultsOnPage, expectedResultOnPage);
        }
    }
}
