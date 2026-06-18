using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestEvents.Application.Exceptions
{
    public class CreateTokenException(string message) : Exception (message);
    {
    }
}
