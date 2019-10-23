namespace PiwotDrawingLib.UI.Controls
{
    class FloatSwitcherControl : SwitcherControl
    {
        private float value = 0;

        /// <summary>
        /// The value of this control.
        /// </summary>
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = (float) System.Math.Round(PiwotToolsLib.PMath.Arit.Clamp(value, min, max), roundingDigits);
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

        protected float step = 1;

        /// <summary>
        /// Magnitude of value increments.
        /// </summary>
        public float Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
            }
        }

        protected float min = 0;

        /// <summary>
        /// The minimal possible value.
        /// </summary>
        public float Min
        {
            get
            {
                return min;
            }
            set
            {
                min = PiwotToolsLib.PMath.Arit.Clamp(value, int.MinValue, max);
                Value = this.value;
                SetPrintableText();
            }
        }

        protected float max = 10;

        /// <summary>
        /// The maximal possible value.
        /// </summary>
        public float Max
        {
            get
            {
                return max;
            }
            set
            {
                max = PiwotToolsLib.PMath.Arit.Clamp(value, min, int.MaxValue);
                Value = this.value;
                SetPrintableText();
            }
        }


        protected string printFormat = "0.00";
        protected int roundingDigits = 2;

        /// <summary>
        /// How many fractional digist should be accounted for.
        /// </summary>
        public int RoundingDigits
        {
            get
            {
                return roundingDigits;
            }
            set
            {
                if(value < 1)
                {
                    throw new System.ArgumentOutOfRangeException();
                }
                roundingDigits = value;
                printFormat = $"0.{PiwotToolsLib.Data.Stringer.GetFilledString(roundingDigits, '0')}";
                SetPrintableText();
            }
        }


        public FloatSwitcherControl(string name, string identificator) : base(name, identificator)
        {

        }

        public FloatSwitcherControl(string name, string identificator, float value, float min, float max, float step) : base(name, identificator)
        {
            Setup(value, min, max, step);
        }

        void Setup(float value, float min, float max, float step)
        {
            if (min > max)
                min = max;
            this.min = min;
            this.max = max;

            Value = value;
            this.step = step;
        }

        /// <summary>
        /// Switches value by one step down.
        /// </summary>
        override public void SwitchLeft()
        {
            if (value > min)
                PerformStep(-1);
            RunActions(new Events.FloatSwitcherEvent(parentMenu, this, value));
        }
        /// <summary>
        /// Switches value by one step up.
        /// </summary>
        override public void SwitchRight()
        {
            if (value < max)
                PerformStep(1);
            RunActions(new Events.FloatSwitcherEvent(parentMenu, this, value));
        }

        override protected void PerformStep(int direction)
        {
            
            Value += step * direction * UpdateFastMultiplier();
        }

        protected override void SetPrintableText()
        {
            string valueText = value.ToString(printFormat);


            if (value == max)
            {
                valueText = (maxSpecialText == "" ? valueText : maxSpecialText);
            }
            else if (value == min)
            {
                valueText = (minSpecialText == "" ? valueText : minSpecialText);
            }

            PrintableText = $"{(hideName ? "" : $"{name}: ")}{(value > min ? LAS : LNS)} {valueText} {(value < max ? RAS : RNS)}";
        }
    }
}