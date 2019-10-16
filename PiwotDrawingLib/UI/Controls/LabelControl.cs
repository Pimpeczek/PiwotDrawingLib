namespace PiwotDrawingLib.UI.Controls
{
    class LabelControl : MenuControl
    {

        public LabelControl(string name, string identificator) : base(name, identificator)
        {
            accessable = false;
        }

        override public void SwitchLeft() { }
        override public void SwitchRight() { }
        override public void Enter() { }

        override public int GetValue() { return 0; }
    }
}