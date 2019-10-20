﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotDrawingLib.UI.Events
{
    class CheckBoxEvent: MenuControllEvent
    {
        public bool Value { get; protected set; }
        public CheckBoxEvent(Containers.Menu menu, Controls.MenuControl controll, bool value) : base(menu, controll)
        {
            Value = value;
        }
    }
}