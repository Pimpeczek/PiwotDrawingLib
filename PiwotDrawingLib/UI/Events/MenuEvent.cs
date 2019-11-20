namespace PiwotDrawingLib.UI.Events
{
    public class MenuEvent
    {
        public string Name { get; protected set; }
        public Containers.StaticMenu Menu { get; protected set; }
        public MenuEvent(Containers.StaticMenu menu)
        {
            Menu = menu;
        }
    }
}