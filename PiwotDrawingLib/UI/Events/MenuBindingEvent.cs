namespace PiwotDrawingLib.UI.Events
{
    class MenuBindingEvent : MenuEvent
    {
        public System.ConsoleKey Key { get; protected set; }
        public MenuBindingEvent(Containers.Menu menu, System.ConsoleKey key) : base(menu)
        {
            Key = key;
        }
    }
}
