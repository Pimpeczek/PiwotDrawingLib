namespace PiwotDrawingLib.UI.Events
{
    public class StringSwitcherEvent : MenuControllEvent
    {
        public int Value { get; protected set; }
        public StringSwitcherEvent(Containers.StaticMenu menu, Controls.StringSwitcherControl controll, int value) : base(menu, controll)
        {
            Value = value;
        }
    }
}