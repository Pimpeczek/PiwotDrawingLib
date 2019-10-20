namespace PiwotDrawingLib.UI.Controls
{
    /// <summary>
    /// Represents a Control used to display text.
    /// </summary>
    class LabelControl : MenuControl
    {

        public LabelControl(string name, string identificator) : base(name, identificator)
        {
            accessable = false;
        }
    }
}