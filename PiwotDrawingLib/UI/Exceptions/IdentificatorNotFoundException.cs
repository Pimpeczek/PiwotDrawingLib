using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Exceptions
{
    public class IdentificatorNotFoundException : Exception
    {
        public IdentificatorNotFoundException() : base()
        {

        }

        public IdentificatorNotFoundException(string message) : base(message)
        {

        }
    }
}
