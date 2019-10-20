namespace PiwotDrawingLib.UI.Events
{
    class IntSwitcherEvent : MenuControllEvent
    {
        public int Value { get; protected set; }
        public IntSwitcherEvent(Containers.Menu menu, Controls.IntSwitcherControl controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}