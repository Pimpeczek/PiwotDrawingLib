namespace PiwotDrawingLib.UI.Events
{
    public class FloatSwitcherEvent : MenuControllEvent
    {
        public float Value { get; protected set; }
        public FloatSwitcherEvent(Containers.StaticMenu menu, Controls.FloatSwitcherControl controll, float value) : base(menu, controll)
        {
            Value = value;
        }
    }
}