using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Domain.Exceptions
{
    /// <summary>
    /// Выбрасывается при неудачном резервировании мест
    /// </summary>
    public class ReserveSeatsException(string message) : Exception(message)
    {
    }
}
