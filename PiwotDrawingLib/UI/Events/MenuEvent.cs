namespace PiwotDrawingLib.UI.Events
{
    class MenuEvent
    {
        public string Name { get; protected set; }
        public Containers.Menu Menu { get; protected set; }
        public MenuEvent(Containers.Menu menu)
        {
            Menu = menu;
        }
    }
}