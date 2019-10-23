namespace PiwotDrawingLib.UI.Controls
{
    class IntSwitcherControl : SwitcherControl
    {
        private int value = 0;

        /// <summary>
        /// The value of this control.
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
                SetPrintableText();
            }
        }

        

        private string minSpecialText = "";

        /// <summary>
        /// Text displayed insted of the value, when it reaches the lower border.
        /// </summary>
        public string MinSpecialText
        {
            get
            {
                return minSpecialText;
            }
            set
            {
                minSpecialText = value;
                SetPrintableText();
            }
        }

        private string maxSpecialText = "";

        /// <summary>
        /// Text displayed insted of the value, when it reaches the upper border.
        /// </summary>
        public string MaxSpecialText
        {
            get
            {
                return maxSpecialText;
            }
            set
            {
                maxSpecialText = value;
                SetPrintableText();
            }
        }


        protected int oryginalStep;
        protected int step;

        /// <summary>
        /// Magnitude of value increments.
        /// </summary>
        public int Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
                oryginalStep = step;
            }
        }

        protected int min = 0;

        /// <summary>
        /// The minimal possible value.
        /// </summary>
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                min = PiwotToolsLib.PMath.Arit.Clamp(value, int.MinValue, max);
                SetPrintableText();
            }
        }

        protected int max = 10;

        /// <summary>
        /// The maximal possible value.
        /// </summary>
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                max = PiwotToolsLib.PMath.Arit.Clamp(value, min, int.MaxValue);
                SetPrintableText();
            }
        }




        public IntSwitcherControl(string name, string identificator) : base(name, identificator)
        {

        }

        public IntSwitcherControl(string name, string identificator, int value, int min, int max, int step) : base(name, identificator)
        {
            Setup(value, min, max, step);
        }

        void Setup(int value, int min, int max, int step)
        {
            if (min > max)
                min = max;
            this.min = min;
            this.max = max;

            Value = value;
            this.step = step;
            this.oryginalStep = step;
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
            
            Value += step * direction * UpdateFastMultiplier();
        }

        protected override void SetPrintableText()
        {
            string valueText;

            if (value == max)
            {
                valueText = (maxSpecialText == "" ? value.ToString() : maxSpecialText);
            }
            else if (value == min)
            {
                valueText = (minSpecialText == "" ? value.ToString() : minSpecialText);
            }
            else
            {
                valueText = value.ToString();
            }

            PrintableText = $"{(hideName ? "" : $"{name}: ")}{(value > min ? LAS : LNS)} {valueText} {(value < max ? RAS : RNS)}";
        }
    }
}