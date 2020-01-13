using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.Drawing.Exceptions
{
    class InvalidCanvasSizeException: Exception
    {
        public Canvas Canvas { get; protected set; }
        public InvalidCanvasSizeException(Canvas canvas) : base() { Canvas = canvas; }
        public InvalidCanvasSizeException(Canvas canvas, string message) : base(message) { Canvas = canvas; }
    }
}
