using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Controls
{
    /// <summary>
    /// Represents pressable control. If ENTER was pressed while this crontrol was highlighted an action will be performed.
    /// </summary>
    public class ButtonControl : ActionControl, IPressable
    {
        public ButtonControl(string name, string identificator):base(name, identificator)
        {
            accessable = true;
        }

        public void Press()
        {
            RunActions(new Events.ButtonEvent(parentMenu, this));
        }
    }
}
