using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Controls
{
    interface Switchable
    {

        /// <summary>
        /// Action invoked when left arrow is pressed over this control.
        /// </summary>
        void SwitchLeft();

        /// <summary>
        /// Action invoked when right arrow is pressed over this control.
        /// </summary>
        void SwitchRight();
    }
}
