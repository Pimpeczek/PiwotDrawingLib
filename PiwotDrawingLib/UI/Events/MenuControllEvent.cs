namespace PiwotDrawingLib.UI.Events
{
    public class MenuControllEvent : MenuEvent
    {
        public Controls.MenuControl Controll { get; protected set; }
        public MenuControllEvent(Containers.StaticMenu menu, Controls.MenuControl controll) : base(menu)
        {
            Controll = controll;
        }
    }
}
