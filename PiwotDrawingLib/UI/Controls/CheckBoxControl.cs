

namespace PiwotDrawingLib.UI.Controls
{
    /// <summary>
    /// Represents pressable and switchable control with true/false value range. If the value was changed while this crontrol was highlighted an action will be performed.
    /// </summary>
    class CheckBoxControl : ActionControl, Switchable, Pressable 
    {
        private int value;

        /// <summary>
        /// The value of this controll. 
        /// <para>1 = true</para> 
        /// <para>0 = false</para> 
        /// </summary>
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = PiwotToolsLib.PMath.Arit.Clamp(value, 0, 1);
                PrintableText = GetPrintableText();
            }
        }
        protected string trueValue = "+";

        /// <summary>
        /// Symbol(string) shown while Value is true.
        /// </summary>
        public string TrueValue
        {
            get
            {
                return trueValue;
            }
            set
            {
                trueValue = value;
                PrintableText = GetPrintableText();
            }
        }

        protected string falseValue = " ";

        /// <summary>
        /// Symbol(string) shown while Value is false.
        /// </summary>
        public string FalseValue
        {
            get
            {
                return falseValue;
            }
            set
            {
                falseValue = value;
                PrintableText = GetPrintableText();
            }
        }

        protected bool hideName = false;

        /// <summary>
        /// If set to 'true' only the value will be shown.
        /// </summary>
        public bool HideName
        {
            get
            {
                return hideName;
            }
            set
            {
                hideName = value;
                PrintableText = GetPrintableText();
            }   
        }

        public CheckBoxControl(string name, string identificator, bool startValue) : base(name, identificator)
        {
            Value = (startValue ? 1 : 0);
            PrintableText = GetPrintableText();
        }

        /// <summary>
        /// Toggles the value and runs actions.
        /// </summary>
        public void SwitchLeft()
        {
            Toggle();
            RunActions(new Events.CheckBoxEvent(parentMenu, this, value));
        }

        /// <summary>
        /// Toggles the value and runs actions.
        /// </summary>
        public void SwitchRight()
        {
            Toggle();
            RunActions(new Events.CheckBoxEvent(parentMenu, this, value));
        }

        /// <summary>
        /// Toggles the value and runs actions.
        /// </summary>
        void Toggle()
        {
            if (value == 0)
                Value = 1;
            else
                Value = 0;
        }

        string GetPrintableText()
        {
            return $"{(hideName ? "" : $"{name}: ")}[{(value == 0 ? falseValue : trueValue)}]";
        }

        /// <summary>
        /// The value of this controll. 
        /// <para>1 = true</para> 
        /// <para>0 = false</para> 
        /// </summary>
        override public int GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Toggles the value and runs actions.
        /// </summary>
        public void Press()
        {
            Toggle();
            RunActions(new Events.CheckBoxEvent(parentMenu, this, value));
        }
    }
}