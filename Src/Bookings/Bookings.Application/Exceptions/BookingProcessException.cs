using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Application.Exceptions
{
    public class BookingProcessException(string message) : Exception(message)
    {
    }
}
