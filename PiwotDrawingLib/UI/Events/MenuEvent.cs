namespace PiwotDrawingLib.UI.Events
{
    public class MenuEvent
    {
        public string Name { get; protected set; }
        public Containers.Menu Menu { get; protected set; }
        public MenuEvent(Containers.Menu menu)
        {
            Menu = menu;
        }
    }
}