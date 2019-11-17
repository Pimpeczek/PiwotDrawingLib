namespace PiwotDrawingLib.UI.Events
{
    public class IntSwitcherEvent : MenuControllEvent
    {
        public int Value { get; protected set; }
        public IntSwitcherEvent(Containers.Menu menu, Controls.IntSwitcherControl controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}