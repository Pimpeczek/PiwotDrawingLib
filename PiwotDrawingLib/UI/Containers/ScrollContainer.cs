using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;
using Pastel;

namespace PiwotDrawingLib.UI.Containers
{
    public class ScrollContainer : Container
    {
        #region Variables

        protected bool showScrolls = true;

        public bool ShowScrolls
        {
            get
            {
                return showScrolls;
            }
            set
            {
                if (showScrolls == value)
                    return;

                showScrolls = value;
                CalculateVisableCanvasSize();
            }
        }

        protected Int2 scrollViewPoint;
        public Int2 ScrollPoint
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
                if (scrollingCanvas != null)
                {
                    scrollingCanvas.ResizeCanvas(new Int2(contentSize.X - 2, scrollingCanvas.Size.Y));
                    CalculateVisableCanvasSize();
                    
                }
                Erase();
                if (IsVIsable)
                    Draw();

            }
        }

        protected Drawing.Canvas scrollingCanvas;
        protected Int2 visableCanvasSize;
        public Int2 ScrollViewSize
        {
            get
            {
                return new Int2(scrollingCanvas.Size);
            }
            set
            {
                if (scrollingCanvas.Size == value)
                    return;
                
                scrollingCanvas.ResizeCanvas(value);
                CalculateVisableCanvasSize();
                Rand.SetSeed(0);
                for (int y = 0; y < scrollingCanvas.Size.Y; y++)
                {
                    for (int x = 0; x < scrollingCanvas.Size.X/2; x++)
                    {
                        scrollingCanvas.DrawOnCanvas(Rand.Int(64) == 0 ? (Rand.Int(16) == 0 ? " ●" : (Rand.Int(8) == 0 ? "+ " : (Rand.Int(4) == 0 ? " *" : " ."))) : "  ", "FFFFCC", "080808", x*2, y) ;
                    }
                }
                scrollingCanvas.ApplyNewFrame();

            }
        }

        protected bool hasVerticalScroll;
        protected bool hasHorizontalScroll;

        #endregion
        public ScrollContainer() : base(new Int2(), new Int2(10, 10), "ScrollView", Misc.Boxes.BoxType.doubled)
        {
            Setup();
        }

        public ScrollContainer(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType) : base(position, size, name, boxType)
        {
            Setup();

        }

        void Setup()
        {
            scrollingCanvas = new Drawing.Canvas(new Int2(contentSize));
            
            visableCanvasSize = new Int2(contentSize);

            ScrollViewSize = new Int2(contentSize);

            scrollViewPoint = Int2.Zero;
        }

        

        /// <summary>
        /// Draws scrollable window.
        /// </summary>
        override protected void DrawWindow()
        {
            base.DrawWindow();
            DrawScrolls();
        }

        protected void DrawScrolls()
        {
            if (!showScrolls)
                return;

            if (hasHorizontalScroll && hasVerticalScroll)
            {
                canvas.DrawOnCanvas(boxCharacters[4], size.X - 3, 0);
                canvas.DrawOnCanvas(boxCharacters[11], 0, size.Y - 3);
                canvas.DrawOnCanvas(boxCharacters[6], visableCanvasSize.X + 1, visableCanvasSize.Y + 1);
                canvas.DrawOnCanvas('¤', visableCanvasSize.X + 2, visableCanvasSize.Y + 2);
                for (int height = 0; height < visableCanvasSize.Y; height++)
                {
                    canvas.DrawOnCanvas(boxCharacters[7], size.X - 3, 1 + height);
                }
                for (int width = 0; width < visableCanvasSize.X; width++)
                {
                    canvas.DrawOnCanvas(boxCharacters[2], 1 + width, size.Y - 3);
                }
            }
            else if (hasVerticalScroll)
            {
                canvas.DrawOnCanvas(boxCharacters[4], size.X - 3, 0);
                canvas.DrawOnCanvas(boxCharacters[10], size.X - 3, size.Y - 1);
                for (int height = 0; height < contentSize.Y; height++)
                {
                    canvas.DrawOnCanvas(boxCharacters[7], size.X - 3, 1 + height);
                }
                
            }
            else
            {
                canvas.DrawOnCanvas(boxCharacters[11], 0, size.Y - 3);
                canvas.DrawOnCanvas(boxCharacters[8], size.X - 1, size.Y - 3);
                for (int width = 0; width < contentSize.X; width++)
                {
                    canvas.DrawOnCanvas(boxCharacters[2], 1 + width, size.Y - 3);
                }
            }

            if (hasVerticalScroll)
            {
                int verticalScrollSpaceLenght = visableCanvasSize.Y + (hasHorizontalScroll ? 1 : 0);

                int verticalScrollPosition = (scrollViewPoint.Y * (verticalScrollSpaceLenght)) * 2 / (ScrollViewSize.Y);

                int verticalScrollSize = Arit.Clamp((int)Math.Ceiling((double)(verticalScrollSpaceLenght * visableCanvasSize.Y * 2) / scrollingCanvas.Size.Y), 1, (verticalScrollSpaceLenght) * 2);

                int bigVerticalScrollSize = (verticalScrollSize + (verticalScrollSize % 2)) / 2;

                for (int i = 0; i < bigVerticalScrollSize; i++)
                {
                    canvas.DrawOnCanvas('█', size.X - 2, verticalScrollPosition / 2 + i + 1);
                }
                if (verticalScrollPosition % 2 == 1)
                {
                    canvas.DrawOnCanvas('▄', size.X - 2, verticalScrollPosition / 2 + 1);
                }
                if ((verticalScrollPosition + verticalScrollSize) % 2 == 1)
                {
                    canvas.DrawOnCanvas('▀', size.X - 2, verticalScrollPosition / 2 + verticalScrollSize / 2 + 1);
                }
            }

            if (hasHorizontalScroll)
            {
                int horizontalScrollSpaceLenght = visableCanvasSize.X + (hasVerticalScroll ? 1 : 0);

                int horizontalScrollPosition = (scrollViewPoint.X * (horizontalScrollSpaceLenght)) * 2 / (ScrollViewSize.X);

                int horizontalScrollSize = Arit.Clamp((int)Math.Ceiling(((double)horizontalScrollSpaceLenght * visableCanvasSize.X * 2) / scrollingCanvas.Size.X), 1, (horizontalScrollSpaceLenght) * 2);

                int bigHorizontalScrollSize = (horizontalScrollSize + (horizontalScrollSize % 2)) / 2;

                for (int i = 0; i < bigHorizontalScrollSize; i++)
                {
                    canvas.DrawOnCanvas('█', horizontalScrollPosition / 2 + i + 1, size.Y - 2);
                }
                if (horizontalScrollPosition % 2 == 1)
                {
                    canvas.DrawOnCanvas('▐', horizontalScrollPosition / 2 + 1, size.Y - 2);
                }
                if ((horizontalScrollPosition + horizontalScrollSize) % 2 == 1)
                {
                    canvas.DrawOnCanvas('▌', horizontalScrollPosition / 2 + horizontalScrollSize / 2 + 1, size.Y - 2);
                }
            }

            


        }

        protected void CalculateVisableCanvasSize()
        {
            visableCanvasSize = new Int2(contentSize);
            if (!showScrolls)
                return;
            hasVerticalScroll = hasHorizontalScroll = false;
            if (visableCanvasSize.Y < ScrollViewSize.Y)
            {
                visableCanvasSize.X -= 2;
                hasVerticalScroll = true;
                if (visableCanvasSize.X < ScrollViewSize.X)
                {
                    hasHorizontalScroll = true;
                    visableCanvasSize.Y -= 2;
                }
                return;
            }

            if (visableCanvasSize.X < ScrollViewSize.X)
            {
                visableCanvasSize.Y -= 2;
                hasHorizontalScroll = true;
                if (visableCanvasSize.Y < ScrollViewSize.Y)
                {
                    hasVerticalScroll = true;
                    visableCanvasSize.X -= 2;
                }
                return;
            }
        }

        public void ScrollUp()
        {
            ScrollBy(Int2.Down);
        }

        public void ScrollDown()
        {
            ScrollBy(Int2.Up);
        }

        public void ScrollLeft()
        {
            ScrollBy(Int2.Left);
        }

        public void ScrollRight()
        {
            ScrollBy(Int2.Right);
        }

        public void ScrollBy(Int2 delta)
        {
            scrollViewPoint = Int2.Clamp(scrollViewPoint + delta, Int2.Clamp(scrollingCanvas.Size - visableCanvasSize, Int2.Zero, Int2.MaxValue));
        }

        protected override void DrawContent()
        {
            canvas.AddCanvas(scrollingCanvas, contentPosition.X, contentPosition.Y, scrollViewPoint.X, scrollViewPoint.Y, visableCanvasSize.X, visableCanvasSize.Y);
        }


    }
}
