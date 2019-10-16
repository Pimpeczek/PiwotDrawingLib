

namespace PiwotDrawingLib.UI.Controls
{
    class CheckBoxControl : MenuControl
    {
        private int value;
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = PiwotToolsLib.PMath.Arit.Clamp(value, 0, 1);
                PrintableText = $"{name}: [{(this.value == 0 ? falseValue : trueValue)}]";
            }
        }
        protected string trueValue = "+";
        public string TrueValue
        {
            get
            {
                return trueValue;
            }
            set
            {
                trueValue = value;
                PrintableText = $"{name}: [{(this.value == 0 ? falseValue : trueValue)}]";
            }
        }

        protected string falseValue = " ";
        public string FalseValue
        {
            get
            {
                return falseValue;
            }
            set
            {
                falseValue = value;
                PrintableText = $"{name}: [{(this.value == 0 ? falseValue : trueValue)}]";
            }
        }

        public CheckBoxControl(string name, string identificator, bool startValue) : base(name, identificator)
        {
            Value = (startValue ? 1 : 0);
        }

        override public void SwitchLeft()
        {
            Toggle();
            RunActions();
        }
        override public void SwitchRight()
        {
            Toggle();
            RunActions();
        }

        public override void Enter()
        {
            Toggle();
            RunActions();
            return;
        }

        void Toggle()
        {
            if (value == 0)
                Value = 1;
            else
                Value = 0;
        }

        override public int GetValue()
        {

            return Value;
        }
    }
}