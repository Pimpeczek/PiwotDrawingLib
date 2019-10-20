using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Events
{
    class ButtonEvent: MenuControllEvent
    {
        public ButtonEvent(Containers.Menu menu, Controls.ButtonControl controll) : base(menu, controll)
        {
        }
    }
}
