namespace PiwotDrawingLib.UI.Controls
{
    class IntSwitcherControl : SwitcherControl
    {
        private int value = 0;
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
            base.step = step;
        }

        override public void SwitchLeft()
        {
            if (value > min)
                PerformStep(-1);
            RunActions(new Events.IntSwitcherEvent(parentMenu, this, value));
        }
        override public void SwitchRight()
        {
            if (value < max)
                PerformStep(1);
            RunActions(new Events.IntSwitcherEvent(parentMenu, this, value));
        }

        override protected void PerformStep(int direction)
        {
            
            if (FastStepTime > 0)
            {
                long timeFromLastSwitch = stopwatch.ElapsedMilliseconds;
                if (timeFromLastSwitch < FastStepTime)
                {
                    fastSwitchCounter++;
                }
                else
                {
                    step = oryginalStep;
                    fastSwitchCounter = 0;
                }
                if (fastSwitchCounter >= FastStepsToMultiply)
                {
                    fastSwitchCounter = 0;
                    step *= FastStepMultiplier;
                }
                stopwatch.Restart();
            }
            Value += step * direction;
        }

        override public int GetValue()
        {
            return Value;
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

            PrintableText = $"{name}: {(value > min ? LAS : LNS)} {valueText} {(value < max ? RAS : RNS)}";
        }
    }
}