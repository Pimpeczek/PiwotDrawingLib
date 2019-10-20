﻿
namespace PiwotDrawingLib.UI.Controls
{
    class StringSwitcherControl : SwitcherControl
    {
        private int value;

        /// <summary>
        /// The index of current option.
        /// </summary>
        public int Value
        {
            get
            {
                return value;
            }
            set
            {

                this.value = PiwotToolsLib.PMath.Arit.Clamp(value, min, max);
                NeedsRedraw = true;
                SetPrintableText();
            }
        }
        protected string[] options;
        public string[] Options
        {
            get
            {
                return options;
            }
            set
            {
                if(value.Length == 0)
                {
                    throw new System.ArgumentException();
                }

                options = value;
                Value = this.value;
            }
        }
        protected new readonly int min;
        protected new int max;

        private new int Min = 0;
        private new int Max = 0;

        private new int Step;

        public StringSwitcherControl(string name, string identificator, string options) : base(name, identificator)
        {

            max = min = 0;
            SetOptions(options);
            Value = value;
            Step = 1;
        }

        /// <summary>
        /// Loads options using string, where each option is separated by a new line symbol.
        /// </summary>
        /// <param name="options"></param>
        public void SetOptions(string options)
        {
            this.options = options.Split('\n');
            max = this.options.Length - 1;
            Value = value;

        }

        /// <summary>
        /// Switches value by one step down.
        /// </summary>
        override public void SwitchLeft()
        {
            if (value > min)
                PerformStep(-1);
            RunActions(new Events.IntSwitcherEvent(parentMenu, this, value));
        }
        /// <summary>
        /// Switches value by one step up.
        /// </summary>
        override public void SwitchRight()
        {
            if (value < max)
                PerformStep(1);
            RunActions(new Events.IntSwitcherEvent(parentMenu, this, value));
        }

        override protected void PerformStep(int direction)
        {
            Value += step * oryginalStep;
            SetPrintableText();
        }

        protected override void SetPrintableText()
        {
            PrintableText = $"{name}: {(value > min ? LAS : LNS)} {options[value]} {(value < max ? RAS : RNS)}";
        }

    }
}