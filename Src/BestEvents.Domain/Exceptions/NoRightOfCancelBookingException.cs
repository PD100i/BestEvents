using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Domain.Exceptions
{
    public class NoRightOfCancelBookingException() : Exception (Messages_ru.NoRightOfCancelBooking)
    {
    }
}
