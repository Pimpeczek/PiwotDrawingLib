using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Events
{
    class CheckBoxEvent: MenuControllEvent
    {
        public int Value { get; protected set; }
        public CheckBoxEvent(Containers.Menu menu, Controls.MenuControl controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}
