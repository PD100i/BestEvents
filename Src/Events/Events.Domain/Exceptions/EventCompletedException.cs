using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Exceptions
{
    /// <summary>
    /// Выбрасывается при попытке забронировать или освободить места на событие, которое уже закончилось
    /// </summary>
    public class EventCompletedException(string message) : ReserveSeatsException(message)
    {
    }
}
