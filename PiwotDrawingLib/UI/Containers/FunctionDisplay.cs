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

        public void RefreshContent()
        {
            DrawContent();
        }

        protected override void DrawContent()
        {
            float height;
            int iHeight;

            float step = 1 / (float)contentSize.X;
            float halfStep = step / 2; 
            for (int x = 0; x < contentSize.X; x++)
            {
                height = (func.Invoke(x * step) * contentSize.Y);
                iHeight = (int)height;
                iHeight = Arit.Clamp(iHeight, contentSize.Y);
                height -= iHeight;
                

                for (int y = 0; y < contentSize.Y - iHeight; y++)
                {
                    Drawing.Renderer.Draw(" ", "FFFFFF", "000000", x + contentPosition.X, y + contentPosition.Y);
                }
                if(height >= 0.5f && iHeight != contentSize.Y)
                {
                    Drawing.Renderer.Draw("▄", "FFFFFF", "000000", x + contentPosition.X, contentSize.Y - iHeight - 1 + contentPosition.Y);//▬
                }
                else if (iHeight == 0 && height >= 0.1f)
                {
                    Drawing.Renderer.Draw("_", "FFFFFF", "000000", x + contentPosition.X, contentSize.Y - 1 + contentPosition.Y);
                }

                for (int y = contentSize.Y - iHeight; y < contentSize.Y; y++)
                {
                    Drawing.Renderer.Draw("█", "FFFFFF", "000000", x + contentPosition.X, y + contentPosition.Y);
                }
            }
            //base.DrawContent();
        }
        public void TestDraw()
        {
            //base.DrawContent();
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Drawing.Renderer.Draw(Name, position.X + (size.X - Name.Length) / 2, position.Y);
        }

    }
}
