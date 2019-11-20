namespace PiwotDrawingLib.UI.Events
{
    public class IntSwitcherEvent : MenuControllEvent
    {
        public int Value { get; protected set; }
        public IntSwitcherEvent(Containers.StaticMenu menu, Controls.IntSwitcherControl controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}