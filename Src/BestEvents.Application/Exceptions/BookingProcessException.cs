using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Application.Exceptions
{
    public class BookingProcessException(string message) : Exception(message)
    {
    }
}
