using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Containers
{
    public class ScrollContainer : Container
    {
        #region Variables

        protected int scrollViewPoint;
        public int ScrollPoint
        {
            get
            {
                return scrollViewPoint;
            }
            set
            {
                if (scrollViewPoint == value)
                    return;

                scrollViewPoint = value;
            }
        }

        public override Int2 Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value < Int2.One * 2)
                    throw new Exceptions.InvalidContainerSizeException(this);


                if (IsVIsable)
                {
                    Erase();
                }
                size = value ?? throw new ArgumentNullException();
                contentSize = size - (boxType != Misc.Boxes.BoxType.none ? Int2.One * 2 : Int2.Zero);
                canvas.ResizeCanvas(size);
                if(scrollingCanvas != null)
                    scrollingCanvas.ResizeCanvas( new Int2(contentSize.X - 2, scrollingCanvas.Size.Y));
                Erase();
                if (IsVIsable)
                    Draw();

            }
        }

        protected Drawing.Canvas scrollingCanvas;

        public int ScrollViewSize
        {
            get
            {
                return scrollingCanvas.Size.Y;
            }
            set
            {
                if (scrollingCanvas.Size.Y == value)
                    return;
                
                scrollingCanvas.ResizeCanvas(new Int2(scrollingCanvas.Size.X-2, value));
                for (int i = 0; i < value; i++)
                {
                    scrollingCanvas.DrawOnCanvas($"{i} ", 0, i);
                }
                scrollingCanvas.ApplyNewFrame();


            }
        }


        #endregion
        public ScrollContainer() : base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            Setup();
        }

        public ScrollContainer(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType) : base(position, size, name, boxType)
        {
            Setup();
        }

        void Setup()
        {
            scrollingCanvas = new Drawing.Canvas(new Int2(contentSize.X-2, contentSize.Y));
            ScrollViewSize = contentSize.Y;
            
        }

        /// <summary>
        /// Draws scrollable window.
        /// </summary>
        override protected void DrawWindow()
        {
            base.DrawWindow();
            DrawScroll();
        }

        protected void DrawScroll()
        {

            canvas.DrawOnCanvas(Misc.Boxes.doubleBoxChars[4], size.X - 3, 0);
            canvas.DrawOnCanvas(Misc.Boxes.doubleBoxChars[10], size.X - 3, size.Y-1);
            for(int height = 0; height < contentSize.Y; height++)
            {
                canvas.DrawOnCanvas(Misc.Boxes.doubleBoxChars[7], size.X - 3, 1 + height);
            }
            int scroolPosition = (scrollViewPoint * contentSize.Y) * 2 / (ScrollViewSize);

            int scrollSize = Arit.Clamp((int)Math.Ceiling((double)(contentSize.Y * contentSize.Y * 2)  / scrollingCanvas.Size.Y), 1, (contentSize.Y) * 2);


            int bigScrollSize = (scrollSize + (scrollSize % 2)) / 2;
            for (int i = 0; i < bigScrollSize; i++)
            {
                canvas.DrawOnCanvas('█', size.X - 2, scroolPosition / 2 + i + 1);
            }
            if (scroolPosition % 2 == 1)
            {
                canvas.DrawOnCanvas('▄', size.X - 2, scroolPosition/2 + 1);
            }
            if ((scroolPosition + scrollSize) % 2 == 1)
            {
                canvas.DrawOnCanvas('▀', size.X - 2, scroolPosition/2 + scrollSize / 2 + 1);
            }


        }

        public void ScrollUp()
        {
            ScrollBy(-1);
        }

        public void ScrollDown()
        {
            ScrollBy(1);
        }

        public void ScrollBy(int delta)
        {
            scrollViewPoint = Arit.Clamp(scrollViewPoint + delta, scrollingCanvas.Size.Y - contentSize.Y);
        }

        protected override void DrawContent()
        {
            canvas.AddCanvas(scrollingCanvas, contentPosition.X, contentPosition.Y, 0, scrollViewPoint, scrollingCanvas.Size.X, contentSize.Y);
        }


    }
}
