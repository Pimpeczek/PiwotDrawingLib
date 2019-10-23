namespace PiwotDrawingLib.UI.Controls
{
    /// <summary>
    /// Represents a control that is not accesable nor visable. Used to separate other controls.
    /// </summary>
    class LineSeparatorControl : MenuControl
    {

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = "";
                PrintableText = "";
            }
        }

        public LineSeparatorControl(string name, string identificator) : base(name, identificator)
        {
            visable = false;
        }
    }
}