namespace PiwotDrawingLib.UI.Controls
{
    class LabelControl : MenuControl
    {

        public LabelControl(string name, string identificator) : base(name, identificator)
        {
            accessable = false;
        }

        override public int GetValue() { return 0; }
    }
}