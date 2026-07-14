using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Exceptions
{
    /// <summary>
    /// Выбрасывается при попытке зарезервировать или освободить места события, которое уже началось
    /// </summary>
    public class EventBegunException(string message) : ReserveSeatsException(message)
    {
    }
}
