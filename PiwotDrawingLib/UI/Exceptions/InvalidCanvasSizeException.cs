using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Exceptions
{
    public class InvalidCanvasSizeException : Exception
    {
        public Containers.Canvas Canvas { get; protected set; }
        public InvalidCanvasSizeException(Containers.Canvas canvas) : base() { Canvas = canvas; }

        public InvalidCanvasSizeException(Containers.Canvas canvas, string message) : base(message) { Canvas = canvas; }
    }
    

}
