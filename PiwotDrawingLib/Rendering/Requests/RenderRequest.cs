using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.Rendering.Requests
{

    /// <summary>
    /// Class representing request for a string to be printed.
    /// </summary>
    public class RenderRequest
    {
        /// <summary>
        /// String to be printed.
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// String to be printed stripped of Pastel wrapper.
        /// </summary>
        public string RawText { get; protected set; }
        /// <summary>
        /// Horisontal position.
        /// </summary>
        public int X { get; protected set; }
        /// <summary>
        /// Vertical position; 0 being on top.
        /// </summary>
        public int Y { get; protected set; }
        public RenderRequest(string text, int x, int y)
        {
            Text = text;
            RawText = Renderer.UnwrapPastel(text);
            X = x;
            Y = y;
        }
    }
}
