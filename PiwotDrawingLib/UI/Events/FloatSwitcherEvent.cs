﻿namespace PiwotDrawingLib.UI.Events
{
    class FloatSwitcherEvent : MenuControllEvent
    {
        public float Value { get; protected set; }
        public FloatSwitcherEvent(Containers.Menu menu, Controls.FloatSwitcherControl controll, float value) : base(menu, controll)
        {
            Value = value;
        }
    }
}