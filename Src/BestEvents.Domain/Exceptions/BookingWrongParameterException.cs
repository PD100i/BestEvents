using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Domain.Exceptions
{
    /// <summary>
    /// Выбрасывается при попытке бронирования с некорректными данными
    /// </summary>
    public class BookingWrongParameterException : Exception
    {
        public BookingWrongParameterException(string message) : base(message)
        {
        }
    }
}
