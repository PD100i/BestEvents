using BestEvents.Domain;
using BestEvents.Presentation;
using BestEvents.Application;

namespace BestEventsTest
{
    public class DtoMapperTest
    {
        [Fact]
        public void MapEventToEventInfoDto_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new DtoMapper();
            var _event = Event.CreateInstanceEvent(Guid.NewGuid(), "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), "Description", 100, 50);
            // Act
            var eventInfoDto = mapper.MapEventToEventInfoDto(_event);
            // Assert
            Assert.Equal(_event.Id.ToString(), eventInfoDto.Id);
            Assert.Equal(_event.Title, eventInfoDto.Title);
            Assert.Equal(_event.StartAt, eventInfoDto.StartAt);
            Assert.Equal(_event.EndAt, eventInfoDto.EndAt);
            Assert.Equal(_event.Description, eventInfoDto.Description);
            Assert.Equal(_event.TotalSeats, eventInfoDto.TotalSeats);
            Assert.Equal(_event.AvailableSeats, eventInfoDto.AvailableSeats);
        }

        [Fact]
        public void MapEventInfoDtoToEvent_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new DtoMapper();
            var eventInfoDto = new EventInfoDto()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Test Event",
                StartAt = DateTime.UtcNow,
                EndAt = DateTime.UtcNow.AddHours(2),
                Description = "Description",
                TotalSeats = 100,
                AvailableSeats = 50
            };
            // Act
            var _event = mapper.MapEventInfoDtoToEvent(eventInfoDto);
            // Assert
            Assert.Equal(Guid.Parse(eventInfoDto.Id), _event.Id);
            Assert.Equal(eventInfoDto.Title, _event.Title);
            Assert.Equal(eventInfoDto.StartAt, _event.StartAt);
            Assert.Equal(eventInfoDto.EndAt, _event.EndAt);
            Assert.Equal(eventInfoDto.Description, _event.Description);
            Assert.Equal(eventInfoDto.TotalSeats, _event.TotalSeats);
            Assert.Equal(eventInfoDto.AvailableSeats, _event.AvailableSeats);
        }

        [Fact]
        public void MapCreateEventDtoToEvent_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new DtoMapper();
            var dto = new CreateEventDto()
            {
                Title = "Test Event",
                StartAt = DateTime.UtcNow,
                EndAt = DateTime.UtcNow.AddHours(2),
                Description = "Description",
                TotalSeats = 100
            };

            // Act
            var _event = mapper.MapCreateEventDtoToEvent(dto);

            // Assert
            Assert.NotEqual(_event.Id, default);
            Assert.Equal(_event.Title, dto.Title);
            Assert.Equal(_event.StartAt, dto.StartAt);
            Assert.Equal(_event.EndAt, dto.EndAt);
            Assert.Equal(_event.Description, dto.Description);
            Assert.Equal(_event.TotalSeats, dto.TotalSeats);
            Assert.Equal(_event.AvailableSeats, dto.TotalSeats);
        }

        [Fact]
        public void MapPaginatedResultToPaginatedResultDto()
        {
            // Arrange
            var mapper = new DtoMapper();
            var collection = EventCollection.GetEventCollection();
            PaginatedResult<Event> paginatedResult = new PaginatedResult<Event>(collection, 1, 100);

            // Act
            var dto = mapper.MapPaginatedResultToPaginationResultDto(paginatedResult);

            // Assert
            Assert.Equal(dto.TotalResultsNumber, paginatedResult.TotalResultsNumber);
            Assert.Equal(dto.ResultsNumberOnPage, paginatedResult.ResultsNumberOnPage);
            Assert.Equal(dto.CurrentPage, paginatedResult.CurrentPage);

            var expectedResultOnPage = paginatedResult.ResultsOnPage.Select(e => new EventInfoDto()
            {
                Id = e.Id.ToString(),
                Title = e.Title,
                Description = e.Description,
                StartAt = e.StartAt,
                EndAt = e.EndAt,
                TotalSeats = e.TotalSeats,
                AvailableSeats = e.AvailableSeats
            })
                .ToList();
            Assert.Equal(dto.ResultsOnPage, expectedResultOnPage);
        }

        [Fact]
        public void MapBookingToBookingDto_ShouldMapCorrectly()
        {
            // Arrange
            var mapper = new DtoMapper();
            var booking = new Booking 
            { 
                Id = Guid.NewGuid(), 
                EventId = Guid.NewGuid(), 
                Status = BookingStatus.Confirmed, 
                CreatedAt = DateTime.UtcNow, 
                ProcessedAt = DateTime.UtcNow.AddMinutes(5) };
            // Act
            var resultDto = mapper.MapBookingToBookingResultDto(booking);
            // Assert
            Assert.Equal(booking.Id.ToString(), resultDto.Id);
            Assert.Equal(booking.EventId.ToString(), resultDto.EventId);
            Assert.Equal(booking.Status.ToString(), resultDto.Status);
            Assert.Equal(booking.CreatedAt, resultDto.CreatedAt);
            Assert.Equal(booking.ProcessedAt, resultDto.ProcessedAt);
        }
    }
}
