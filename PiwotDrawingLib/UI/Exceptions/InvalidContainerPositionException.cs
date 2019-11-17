using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Exceptions
{
    public class InvalidContainerPositionException : Exception
    {
        public Containers.Container Container { get; protected set; }
        public InvalidContainerPositionException(Containers.Container container) : base() { Container = container; }

        public InvalidContainerPositionException(Containers.Container container, string message) : base(message) { Container = container; }
    }
}
