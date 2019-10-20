namespace PiwotDrawingLib.UI.Events
{
    class StringSwitcherEvent : MenuControllEvent
    {
        public int Value { get; protected set; }
        public StringSwitcherEvent(Containers.Menu menu, Controls.StringSwitcherControl controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}