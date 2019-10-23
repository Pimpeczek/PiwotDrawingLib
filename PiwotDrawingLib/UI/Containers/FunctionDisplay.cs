using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Containers
{
    class FunctionDisplay : Container
    {
        protected float domainLowerBorder = 0;
        public float DomainLowerBorder
        {
            get
            {
                return domainLowerBorder;
            }
            set
            {
                domainLowerBorder = Arit.Clamp(value, float.MinValue, domainUpperBorder);
            }
        }

        protected float domainUpperBorder = 1;
        public float DomainUpperBorder
        {
            get
            {
                return domainUpperBorder;
            }
            set
            {
                domainLowerBorder = Arit.Clamp(value, domainLowerBorder, float.MaxValue);
            }
        }

        protected Func<float, float> func;

        public Func<float, float> Function
        {
            get
            {
                return func;
            }
            set
            {
                if (func == null)
                    throw new ArgumentNullException();
                func = value;
            }
        }

        public FunctionDisplay() : base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            IsVIsable = false;
            Setup();
            IsVIsable = true;
        }

        public FunctionDisplay(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType, Func<float, float> func) : base(position, size, name, boxType)
        {
            Setup();
        }

        void Setup()
        {


        }

        protected override void DrawContent()
        {
            throw new NotImplementedException();
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);
        }

    }
}
