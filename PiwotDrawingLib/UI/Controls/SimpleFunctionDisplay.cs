using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiwotToolsLib.PMath;
using PiwotDrawingLib.Drawing;

namespace PiwotDrawingLib.UI.Controls
{
    public class SimpleFunctionDisplay : UIElement
    {
        #region Variables

        protected Func<float, float> func;

        /// <summary>
        /// A function of x with domain and values in range [0, 1].
        /// </summary>
        public Func<float, float> Function
        {
            get
            {
                return func;
            }
            set
            {
                func = value ?? throw new ArgumentNullException();
            }
        }
        protected float[] values;
        #endregion
        public SimpleFunctionDisplay() : base()
        {
            Setup(new Int2(), new Int2(10, 10), (x) => x);
        }

        public SimpleFunctionDisplay(Int2 position, Int2 size, Func<float, float> func) : base()
        {
            Setup(position, size, func);
        }

        void Setup(Int2 position, Int2 size, Func<float, float> func)
        {
            Position = position;
            Size = size;
            this.func = func;
        }

        public override void PrintOnCanvas(Canvas canvas)
        {
            float height;
            int iHeight;
            if (values == null)
            {
                values = new float[size.X];
            }

            float step = 1 / (float)size.X;
            values = PiwotToolsLib.Data.Arrays.BuildArray(size.X, (x) => func.Invoke(x * step));
            float maxval = 0;
            int maxvalid = 0;
            for (int x = 0; x < size.X; x++)
            {
                if (values[x] > maxval)
                {
                    maxval = values[x];
                    maxvalid = x;
                }
            }
            float halfStep = step / 2;
            string forwardCol = "";
            for (int x = 0; x < size.X; x++)
            {
                height = values[x] * size.Y;
                iHeight = (int)height;
                iHeight = Arit.Clamp(iHeight, size.Y);
                height -= iHeight;
                forwardCol = (x == maxvalid ? "FF0000" : "FFFFFF");

                for (int y = 0; y < size.Y - iHeight; y++)
                {
                    canvas.DrawOnCanvas(" ", forwardCol, "000000", 1 + x, 1 + y);
                }
                if (height >= 0.5f && iHeight != size.Y)
                {
                    canvas.DrawOnCanvas("▄", forwardCol, "000000", 1 + x, 1 + size.Y - iHeight - 1);//▬
                }
                else if (iHeight == 0 && height >= 0.1f)
                {
                    canvas.DrawOnCanvas("_", forwardCol, "000000", 1 + x, 1 + size.Y - 1);
                }

                for (int y = size.Y - iHeight; y < size.Y; y++)
                {
                    canvas.DrawOnCanvas("█", forwardCol, "000000", 1 + x, 1 + y);
                }
            }
        }
    }
}
