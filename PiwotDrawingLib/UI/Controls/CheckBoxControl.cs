

namespace PiwotDrawingLib.UI.Controls
{
    /// <summary>
    /// Represents pressable and switchable control with true/false value range. If the value was changed while this crontrol was highlighted an action will be performed.
    /// </summary>
    public class CheckBoxControl : ActionControl, ISwitchable, IPressable
    {
        private bool value;
        /// <summary>
        /// The value of this controll. 
        /// <para>1 = true</para> 
        /// <para>0 = false</para> 
        /// </summary>
        public bool Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                SetPrintableText();
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
                SetPrintableText();
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
                SetPrintableText();
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
                SetPrintableText();
            }   
        }

        public CheckBoxControl(string name, string identificator, bool startValue) : base(name, identificator)
        {
            Value = startValue;
            SetPrintableText();
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
            Value = !value;
        }

        protected void SetPrintableText()
        {
            PrintableText = $"{(hideName ? "" : $"{name}: ")}[{(value ? trueValue : falseValue)}]";
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