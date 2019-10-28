using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Exceptions
{
    class InvalidContainerSizeException : Exception
    {
        public Containers.Container Container { get; protected set; }
        public InvalidContainerSizeException(Containers.Container container) : base() { Container = container; }

        public InvalidContainerSizeException(Containers.Container container, string message) : base(message) { Container = container; }
    }
    

}
