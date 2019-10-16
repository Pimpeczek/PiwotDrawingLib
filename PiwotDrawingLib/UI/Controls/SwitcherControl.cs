using System.Diagnostics;

namespace PiwotDrawingLib.UI.Controls
{
    abstract class SwitcherControl : MenuControl
    {

        protected string LAS;
        protected string RAS;
        protected string LNS;
        protected string RNS;
        
        public string LeftAvlaiableSymbol
        {
            get
            {
                return LAS;
            }
            set
            {
                LAS = value;
                SetPrintableText();
            }
        }

        public string RightAvaliableSymbol
        {
            get
            {
                return RAS;
            }
            set
            {
                RAS = value;
                SetPrintableText();
            }
        }

        public string LeftNonavaliableSymbol
        {
            get
            {
                return LNS;
            }
            set
            {
                LNS = value;
                SetPrintableText();
            }
        }

        public string RightNonavaliableSymbol
        {
            get
            {
                return RNS;
            }
            set
            {
                RNS = value;
                SetPrintableText();
            }
        }

        protected int fastSwitchCounter;
        public int FastStepTime { get; set; }
        public int FastStepMultiplier { get; set; }
        public int FastStepsToMultiply { get; set; }


        protected int oryginalStep;
        protected int step;
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
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                this.min = PiwotToolsLib.PMath.Arit.Clamp(value, int.MinValue, max);
                SetPrintableText();
            }
        }

        protected int max = 10;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                this.max = PiwotToolsLib.PMath.Arit.Clamp(value, min, int.MaxValue);
                SetPrintableText();
            }
        }

        protected Stopwatch stopwatch;

        public SwitcherControl(string name, string identificator) : base(name, identificator)
        {
            LAS = "◄";
            RAS = "►";
            LNS = " ";
            RNS = " ";
            fastSwitchCounter = 0;
            FastStepTime = 0;
            FastStepMultiplier = 10;
            FastStepsToMultiply = 10;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        abstract protected void PerformStep(int direction);

        abstract protected void SetPrintableText();
    }
}
