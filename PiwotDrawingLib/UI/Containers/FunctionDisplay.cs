using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Containers
{
    class FunctionDisplay : Canvas
    {
        #region Variables

        protected Func<float, float> func;

        /// <summary>
        /// A function of x with domain in range [0, 1] and values in range [0, 1].
        /// </summary>
        public Func<float, float> Function
        {
            get
            {
                return func;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                func = value;
            }
        }
        #endregion
        public FunctionDisplay() : base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            IsVIsable = false;
            Setup((x)=>x);
            IsVIsable = true;
        }

        public FunctionDisplay(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType, Func<float, float> func) : base(position, size, name, boxType)
        {
            Setup(func);
        }

        void Setup(Func<float, float> func)
        {

            this.func = func;
        }

        public override void RefreshContent()
        {
            DrawContent();
        }

        protected override void DrawContent()
        {
            float height;
            int iHeight;
            bool halfFlag;
            bool tehthFlag;
            float step = 1 / (float)canvasSize.X;
            float halfStep = step / 2;
            for (int x = 0; x < canvasSize.X; x++)
            {
                height = (func.Invoke(x * step) * canvasSize.Y);
                iHeight = (int)height;
                iHeight = Arit.Clamp(iHeight, canvasSize.Y-1);
                height -= iHeight;
                

                for (int y = 0; y < canvasSize.Y - iHeight; y++)
                {
                    WriteOnCanvas(" ", defFHex, defBHex, x, y);
                }
                if(height >= 0.5f)
                {
                    WriteOnCanvas("▄", defFHex, defBHex, x, canvasSize.Y - iHeight - 1);
                }
                else if (iHeight == 0 && height >= 0.1f)
                {
                    WriteOnCanvas("_", defFHex, defBHex, x, canvasSize.Y - iHeight - 1);
                }

                for (int y = canvasSize.Y - iHeight; y < canvasSize.Y; y++)
                {
                    WriteOnCanvas("█", defFHex, defBHex, x, y);
                }
            }
            base.DrawContent();
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);
        }

    }
}
