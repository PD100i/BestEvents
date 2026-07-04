using Bookings.Domain;
using Bookings.Domain.Exceptions;
using Bookings.Application;
using Riok.Mapperly.Abstractions;


namespace Bookings.Presentation
{
    /// <summary>
    /// Маппер для преобразования между доменными моделями и DTO
    /// </summary>
    [Mapper]
    public partial class DtoMapper
    {
        

        /// <summary>
        /// Мапинг из доменной модели Booking в DTO BookingResultDto для передачи данных о бронировании в контроллере и отображения пользователю.
        /// </summary>
        /// <param name="booking">Доменная модель бронирования</param>
        /// <returns>DTO модель результата бронирования</returns>
        [MapperIgnoreSource(nameof(Booking.UserId))]
        public partial BookingResultDto MapBookingToBookingResultDto(Booking booking);

        /// <summary>
        /// Мапинг из строки в Guid с проверкой формата. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="EventWrongParameterException"></exception>
        public Guid StringToGuid(string id)
        {
            return Guid.TryParse(id, out Guid result) ? result : throw new BookingWrongParameterException(Messages_ru.WrongIdFormat);
        }

    }
}
