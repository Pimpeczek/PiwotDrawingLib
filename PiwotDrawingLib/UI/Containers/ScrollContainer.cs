using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;
using Pastel;
using PiwotDrawingLib.Drawing;

namespace PiwotDrawingLib.UI.Containers
{
    public class ScrollContainer : Container
    {
        #region Variables

        public enum ContentHandling
        {
            /// <summary>
            /// 
            /// </summary>
            FitScrollViewToContainer,

            /// <summary>
            /// Resizes ScrollView to fit all children and keeps it that way for new children.
            /// </summary>
            FitScrollViewToChildren,

            /// <summary>
            /// Resizes container to fit the scrollview.
            /// </summary>
            FitContainerViewToScrollView
        }

        protected ContentHandling contentFittingMode;


        public ContentHandling ContentFittingMode
        {
            get
            {
                return contentFittingMode;
            }
            set
            {
                if (contentFittingMode == value)
                    return;

                WaitForDrawingEnd();

                contentFittingMode = value;
                switch(contentFittingMode)
                {
                    case ContentHandling.FitScrollViewToContainer:
                        scrollingCanvas.ResizeCanvas(scrollingCanvas.Size);
                        break;
                    case ContentHandling.FitScrollViewToChildren:
                        ResizeScrollviewToChildren();
                        break;

                }

            }
        }

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

                WaitForDrawingEnd();

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

                WaitForDrawingEnd();

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

                WaitForDrawingEnd();

                size = value ?? throw new ArgumentNullException();
                contentSize = size - (boxType != Misc.Boxes.BoxType.none ? Int2.One * 2 : Int2.Zero);
                canvas.ResizeCanvas(size);
                


                if (boxType == Misc.Boxes.BoxType.none)
                {
                    scrollsSizes = Int2.One;
                    verticalScrollFieldPosition = new Int2(ContentSize.X + contentPosition.X - 1, 0);
                    horisontalScrollFieldPosition = new Int2(0, ContentSize.Y + contentPosition.Y - 1);
                }
                else
                {
                    scrollsSizes = Int2.One * 3;
                    verticalScrollFieldPosition = new Int2(ContentSize.X + contentPosition.X - 1, 1);
                    horisontalScrollFieldPosition = new Int2(1, ContentSize.Y + contentPosition.Y - 1);
                }

                if (scrollingCanvas != null)
                {
                    CalculateVisableCanvasSize();
                }


                if (IsVIsable)
                {
                    Clear();
                    Draw();
                }

            }
        }

        protected Drawing.Canvas scrollingCanvas;
        protected Int2 visableCanvasSize;
        protected Int2 scrollsSizes;
        protected Int2 verticalScrollFieldPosition;
        protected Int2 horisontalScrollFieldPosition;
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
                WaitForDrawingEnd();
                scrollingCanvas.ResizeCanvas(value);
                CalculateVisableCanvasSize();
                /*Rand.SetSeed(0);
                for (int y = 0; y < scrollingCanvas.Size.Y; y++)
                {
                    for (int x = 0; x < scrollingCanvas.Size.X; x++)
                    {
                        scrollingCanvas.DrawOnCanvas(Rand.Int(64) == 0 ? (Rand.Int(16) == 0 ? " ●" : (Rand.Int(8) == 0 ? "+ " : (Rand.Int(4) == 0 ? " *" : " ."))) : "  ", "FFFFCC", "080808", x*2, y) ;
                    }
                }
                scrollingCanvas.ApplyNewFrame();
                */
            }
        }

        protected bool hasVerticalScroll;
        protected bool hasHorizontalScroll;

        #endregion
        public ScrollContainer() : base(new Int2(), new Int2(10, 10), "ScrollView", Misc.Boxes.BoxType.doubled)
        {
            Setup(new Int2(10, 10), Misc.Boxes.BoxType.doubled);
        }

        public ScrollContainer(Int2 position, Int2 size, Int2 scrollViewSize, string name, Misc.Boxes.BoxType boxType) : base(position, size, name, boxType)
        {
            Setup(scrollViewSize, boxType);

        }

        void Setup(Int2 scrollViewSize, Misc.Boxes.BoxType boxType)
        {
            scrollsSizes = boxType == Misc.Boxes.BoxType.none ? Int2.One : Int2.One * 3;

            scrollingCanvas = new Drawing.Canvas(new Int2(scrollViewSize));

            //visableCanvasSize = new Int2(contentSize);

            ScrollViewSize = scrollViewSize;
            scrollViewPoint = Int2.Zero;
            CalculateVisableCanvasSize();

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
                if (BoxType != Misc.Boxes.BoxType.none)
                {
                    canvas.DrawOnCanvas(boxCharacters[4], size.X - scrollsSizes.X, 0);
                    canvas.DrawOnCanvas(boxCharacters[11], 0, size.Y - scrollsSizes.Y);
                    canvas.DrawOnCanvas(boxCharacters[6], visableCanvasSize.X + 1, visableCanvasSize.Y + 1);
                    
                    for (int height = 0; height < visableCanvasSize.Y; height++)
                    {
                        canvas.DrawOnCanvas(boxCharacters[7], size.X - scrollsSizes.X, 1 + height);
                        canvas.DrawOnCanvas(' ', size.X - 2, 1 + height);
                    }
                    for (int width = 0; width < visableCanvasSize.X; width++)
                    {
                        canvas.DrawOnCanvas(boxCharacters[2], 1 + width, size.Y - scrollsSizes.Y);
                        canvas.DrawOnCanvas(' ', 1 + width, size.Y - 2);
                    }
                    canvas.DrawOnCanvas(' ', 1 + visableCanvasSize.X, size.Y - 2);
                    canvas.DrawOnCanvas(' ', size.X - 2, 1 + visableCanvasSize.Y);
                }
                else
                {
                    for (int height = 0; height < ContentSize.Y; height++)
                    {
                        canvas.DrawOnCanvas(' ', verticalScrollFieldPosition.X, height);
                    }
                    for (int width = 0; width < ContentSize.X; width++)
                    {
                        canvas.DrawOnCanvas(' ', width, horisontalScrollFieldPosition.Y);
                    }
                }
                canvas.DrawOnCanvas('¤', verticalScrollFieldPosition.X, horisontalScrollFieldPosition.Y);

            }
            else if (hasVerticalScroll)
            {
                
                if (BoxType != Misc.Boxes.BoxType.none)
                {
                    canvas.DrawOnCanvas(boxCharacters[4], size.X - scrollsSizes.X, 0);
                    canvas.DrawOnCanvas(boxCharacters[10], size.X - scrollsSizes.X, size.Y - 1);
                    for (int height = 0; height < contentSize.Y; height++)
                    {
                        canvas.DrawOnCanvas(boxCharacters[7], size.X - scrollsSizes.X, 1 + height);
                        canvas.DrawOnCanvas(' ', size.X - 2, 1 + height);
                    }
                }
                else
                {
                    for (int height = 0; height < visableCanvasSize.Y; height++)
                    {
                        canvas.DrawOnCanvas(' ', verticalScrollFieldPosition.X, height);
                    }
                }

            }
            else if (hasHorizontalScroll)
            {
                if (BoxType != Misc.Boxes.BoxType.none)
                {
                    canvas.DrawOnCanvas(boxCharacters[11], 0, size.Y - scrollsSizes.Y);
                    canvas.DrawOnCanvas(boxCharacters[8], size.X - 1, size.Y - scrollsSizes.Y);
                    for (int width = 0; width < contentSize.X; width++)
                    {
                        canvas.DrawOnCanvas(boxCharacters[2], 1 + width, size.Y - scrollsSizes.Y);
                        canvas.DrawOnCanvas(' ', 1 + width, size.Y - 2);
                    }
                }
                else
                {
                    for (int width = 0; width < visableCanvasSize.X; width++)
                    {
                        canvas.DrawOnCanvas(' ', width, horisontalScrollFieldPosition.Y);
                    }
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
                    canvas.DrawOnCanvas('█', verticalScrollFieldPosition.X, verticalScrollPosition / 2 + i + verticalScrollFieldPosition.Y);
                }
                if (verticalScrollPosition % 2 == 1)
                {
                    canvas.DrawOnCanvas('▄', verticalScrollFieldPosition.X, verticalScrollPosition / 2 + verticalScrollFieldPosition.Y);
                }
                if ((verticalScrollPosition + verticalScrollSize) % 2 == 1)
                {
                    canvas.DrawOnCanvas('▀', verticalScrollFieldPosition.X, verticalScrollPosition / 2 + verticalScrollSize / 2 + verticalScrollFieldPosition.Y);
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
                    canvas.DrawOnCanvas('█', horizontalScrollPosition / 2 + i + horisontalScrollFieldPosition.X, horisontalScrollFieldPosition.Y);
                }
                if (horizontalScrollPosition % 2 == 1)
                {
                    canvas.DrawOnCanvas('▐', horizontalScrollPosition / 2 + horisontalScrollFieldPosition.X, horisontalScrollFieldPosition.Y);
                }
                if ((horizontalScrollPosition + horizontalScrollSize) % 2 == 1)
                {
                    canvas.DrawOnCanvas('▌', horizontalScrollPosition / 2 + horizontalScrollSize / 2 + horisontalScrollFieldPosition.X, horisontalScrollFieldPosition.Y);
                }
            }

            


        }


        public override void Draw()
        {
            IsVIsable = true;
            DrawWindow();
            DrawContent();
            canvas.ApplyNewFrame();
            if (parent == null)
            {
                Drawing.Renderer.Draw(canvas, position.X, position.Y);
            }
        }

        protected void CalculateVisableCanvasSize()
        {
            visableCanvasSize = new Int2(contentSize);
            if (!showScrolls)
                return;
            hasVerticalScroll = hasHorizontalScroll = false;
            int bordersAddWidth = BoxType == Misc.Boxes.BoxType.none ? 0 : 2;
            
            if (visableCanvasSize.Y < ScrollViewSize.Y)
            {
                visableCanvasSize.X -= bordersAddWidth;
                hasVerticalScroll = true;
                if (visableCanvasSize.X < ScrollViewSize.X)
                {
                    hasHorizontalScroll = true;
                    
                    visableCanvasSize.Y -= bordersAddWidth;
                }
                return;
            }

            if (visableCanvasSize.X < ScrollViewSize.X)
            {
                visableCanvasSize.Y -= bordersAddWidth;
                hasHorizontalScroll = true;
                
                if (visableCanvasSize.Y < ScrollViewSize.Y)
                {
                    hasVerticalScroll = true;
                    visableCanvasSize.X -= bordersAddWidth;
                }
                return;
            }
        }

        public void ScrollUp()
        {
            WaitForDrawingEnd();
            ScrollBy(Int2.Down);
        }

        public void ScrollDown()
        {
            WaitForDrawingEnd();
            ScrollBy(Int2.Up);
        }

        public void ScrollLeft()
        {
            WaitForDrawingEnd();
            ScrollBy(Int2.Left);
        }

        public void ScrollRight()
        {
            WaitForDrawingEnd();
            ScrollBy(Int2.Right);
        }

        public void ScrollBy(Int2 delta)
        {
            WaitForDrawingEnd();
            scrollViewPoint = Int2.Clamp(scrollViewPoint + delta, Int2.Clamp(scrollingCanvas.Size - visableCanvasSize, Int2.Zero, Int2.MaxValue));
        }

        protected override void DrawContent()
        {
            PrintChildren();
            scrollingCanvas.ApplyNewFrame();
            canvas.AddCanvas(scrollingCanvas, contentLocalPosition.X, contentLocalPosition.Y, scrollViewPoint.X, scrollViewPoint.Y, visableCanvasSize.X, visableCanvasSize.Y);
            
        }

        /// <summary>
        /// Prints children on this Container's canvas.
        /// </summary>
        protected override void PrintChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].Visable)
                {
                    children[i].PrintOnCanvas(scrollingCanvas);
                }
            }
        }

        public override void PrintOnCanvas(Canvas canvas)
        {
            isBeingDrawn = true;
            DrawWindow();
            DrawContent();
            this.canvas.ApplyNewFrame();
            canvas.AddCanvas(this.canvas, position.X, position.Y);
            isBeingDrawn = false;
        }

        public override void AddChild(UIElement element)
        {
            base.AddChild(element);
            if (ContentFittingMode == ContentHandling.FitScrollViewToChildren)
            {
                ResizeScrollviewToChildren();
            }
        }

        public override void RemoveChild(UIElement element)
        {
            base.RemoveChild(element);
            if(ContentFittingMode == ContentHandling.FitScrollViewToChildren)
            {
                ResizeScrollviewToChildren();
            }
        }

        protected void ResizeScrollviewToChildren()
        {
            //TODO!!!!!!!!!!!!!!!!!!!!!
        }

    }
}
