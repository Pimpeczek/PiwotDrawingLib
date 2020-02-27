using Pastel;
using PiwotToolsLib.PMath;
using System;

namespace PiwotDrawingLib.Drawing
{
    public class Canvas
    {
        public string[,] frameFrontColorMap;
        public string[,] frameBackColorMap;
        public char[][] frameCharMap;

        public string[,] canvasFrontColorMap;
        public string[,] canvasBackColorMap;
        public char[][] canvasCharMap;

        public bool[,] refreshMap;

        bool canvasUpToDate;

        bool useColor;

        /// <summary>
        /// <para>If set to true the Renderer will use Pastel library to display colored characters.</para>
        /// <para>If set to false the Renderer will use only default colors to display characters, but will work much faster.</para>
        /// <para>Refresing while canvas might be advised while changing this flag after something was already printed.</para>
        /// </summary>
        public bool UseColor
        {
            get
            {
                return useColor;
            }
            set
            {
                useColor = value;

            }
        }

        public Int2 Size
        {
            get; protected set;
        } 

        public Canvas(Int2 size)
        {
            useColor = true;
            Size = size;
            frameFrontColorMap = new string[Size.Y, Size.X];
            frameBackColorMap = new string[Size.Y, Size.X];
            frameCharMap = new char[Size.Y][];

            canvasFrontColorMap = new string[Size.Y, Size.X];
            canvasBackColorMap = new string[Size.Y, Size.X];
            canvasCharMap = new char[Size.Y][];

            refreshMap = new bool[Size.Y, Size.X + 1];

            for (int i = 0; i < Size.Y; i++)
            {
                frameCharMap[i] = new char[Size.X];

                canvasCharMap[i] = new char[Size.X];
                refreshMap[i, Size.X] = false;
                for (int j = 0; j < Size.X; j++)
                {
                    frameCharMap[i][j] = ' ';
                    frameFrontColorMap[i, j] = Renderer.defFHex;
                    frameBackColorMap[i, j] = Renderer.defBHex;

                    canvasCharMap[i][j] = ' ';
                    canvasFrontColorMap[i, j] = Renderer.defFHex;
                    canvasBackColorMap[i, j] = Renderer.defBHex;

                    refreshMap[i, j] = false;
                }
            }
            canvasUpToDate = false;
        }


        public void ResizeCanvas(Int2 newSize)
        {
            string[,] newFrameFrontColorMap;
            string[,] newFrameBackColorMap;
            char[][] newFrameCharMap;

            string[,] newCanvasFrontColorMap;
            string[,] newCanvasBackColorMap;
            char[][] newCanvasCharMap;


            bool[,] newRefreshMap;
            newFrameFrontColorMap = new string[newSize.Y, newSize.X];
            newFrameBackColorMap = new string[newSize.Y, newSize.X];
            newFrameCharMap = new char[newSize.Y][];

            newCanvasFrontColorMap = new string[newSize.Y, newSize.X];
            newCanvasBackColorMap = new string[newSize.Y, newSize.X];
            newCanvasCharMap = new char[newSize.Y][];

            newRefreshMap = new bool[newSize.Y, newSize.X + 1];

            for (int i = 0; i < newSize.Y && i < Size.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = 0; j < newSize.X && j < Size.X; j++)
                {
                    newFrameCharMap[i][j] = frameCharMap[i][j];
                    newFrameFrontColorMap[i, j] = frameFrontColorMap[i, j];
                    newFrameBackColorMap[i, j] = frameBackColorMap[i, j];

                    newCanvasCharMap[i][j] = canvasCharMap[i][j];
                    newCanvasFrontColorMap[i, j] = canvasFrontColorMap[i, j];
                    newCanvasBackColorMap[i, j] = canvasBackColorMap[i, j];

                    newRefreshMap[i, j] = refreshMap[i, j];
                }
                for (int j = Size.X; j < newSize.X; j++)
                {
                    newFrameCharMap[i][j] = ' ';
                    newFrameFrontColorMap[i, j] = Drawing.Renderer.defFHex;
                    newFrameBackColorMap[i, j] = Drawing.Renderer.defBHex;

                    newCanvasCharMap[i][j] = ' ';
                    newCanvasFrontColorMap[i, j] = Drawing.Renderer.defFHex;
                    newCanvasBackColorMap[i, j] = Drawing.Renderer.defBHex;
                    newRefreshMap[i, j] = false;
                }
            }

            for (int i = Size.Y; i < newSize.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = 0; j < newSize.X; j++)
                {
                    newFrameCharMap[i][j] = ' ';
                    newFrameFrontColorMap[i, j] = Drawing.Renderer.defFHex;
                    newFrameBackColorMap[i, j] = Drawing.Renderer.defBHex;

                    newCanvasCharMap[i][j] = ' ';
                    newCanvasFrontColorMap[i, j] = Drawing.Renderer.defFHex;
                    newCanvasBackColorMap[i, j] = Drawing.Renderer.defBHex;
                    newRefreshMap[i, j] = false;
                }
            }
            Size = new Int2(newSize);
            frameFrontColorMap = (string[,])newFrameFrontColorMap.Clone();
            frameBackColorMap = (string[,])newFrameBackColorMap.Clone();
            frameCharMap = newFrameCharMap;

            canvasFrontColorMap = (string[,])newCanvasFrontColorMap.Clone();
            canvasBackColorMap = (string[,])newCanvasBackColorMap.Clone();
            canvasCharMap = newCanvasCharMap;

            refreshMap = newRefreshMap;
            canvasUpToDate = false;
        }

        public void DrawOnCanvas(string Text, int x, int y)
        {
            DrawOnCanvas(Text, Renderer.defFHex, Renderer.defBHex, x, y);
        }


        public void DrawOnCanvas(string Text, string FID, string BID, int x, int y)
        {
            if (y >= 0 && y < Size.Y)
            {
                for (int i = 0; i < Text.Length && x < Size.X; i++)
                {
                    if (x >= 0)
                    {
                        frameCharMap[y][x] = Text[i];
                        if (useColor)
                        {
                            frameFrontColorMap[y, x] = FID;

                            frameBackColorMap[y, x] = BID;
                        }
                    }
                    x++;
                }
                refreshMap[y, Size.X] = true;
            }
            canvasUpToDate = false;
        }

        public void DrawOnCanvas(char Char, int x, int y)
        {
            DrawOnCanvas(Char, Renderer.defFHex, Renderer.defBHex, x, y);
        }

        public void DrawOnCanvas(char Char, string FID, string BID, int x, int y)
        {

            if (x >= 0 && x < Size.X && y >= 0 && y < Size.Y)
            {
                frameCharMap[y][x] = Char;
                if (useColor)
                {
                    frameFrontColorMap[y, x] = FID;

                    frameBackColorMap[y, x] = BID;
                }
                refreshMap[y, Size.X] = true;
            }
            canvasUpToDate = false;
        }

        public void ApplyNewFrame()
        {
            if (canvasUpToDate)
                return;
            for (int y = 0; y < Size.Y; y++)
            {
                if (refreshMap[y, Size.X])
                {
                    refreshMap[y, Size.X] = false;
                    for (int x = 0; x < Size.X; x++)
                    {
                        UpdateOneCell(x, y);
                    }
                }
            }
            canvasUpToDate = true;
        }

        public void Clear()
        {
            for (int i = 0; i < Size.Y; i++)
            {
                refreshMap[i, Size.X] = false;
                for (int j = 0; j < Size.X; j++)
                {
                    frameCharMap[i][j] = ' ';
                    frameFrontColorMap[i, j] = Renderer.defFHex;
                    frameBackColorMap[i, j] = Renderer.defBHex;

                    canvasCharMap[i][j] = ' ';
                    canvasFrontColorMap[i, j] = Renderer.defFHex;
                    canvasBackColorMap[i, j] = Renderer.defBHex;

                    refreshMap[i, j] = false;
                }
            }
            canvasUpToDate = false;
        }

        public void Clear(Int2 position, Int2 size)
        {
            Clear(position.X, position.Y, size.X, size.Y);
        }

        public void Clear(int x, int y, int width, int height)
        {
            for (int i = y; i < Size.Y && i < height; i++)
            {
                refreshMap[i, Size.X] = false;
                for (int j = x; j < Size.X && j < width; j++)
                {
                    frameCharMap[i][j] = ' ';
                    frameFrontColorMap[i, j] = Renderer.defFHex;
                    frameBackColorMap[i, j] = Renderer.defBHex;

                    canvasCharMap[i][j] = ' ';
                    canvasFrontColorMap[i, j] = Renderer.defFHex;
                    canvasBackColorMap[i, j] = Renderer.defBHex;

                    refreshMap[i, j] = false;
                }
            }
            canvasUpToDate = false;
        }

        private void UpdateOneCell(int x, int y)
        {
            bool tFlag = false;
            if (useColor)
            {
                if (frameFrontColorMap[y, x] != canvasFrontColorMap[y, x])
                {
                    canvasFrontColorMap[y, x] = frameFrontColorMap[y, x];
                    tFlag = true;
                }

                if (frameBackColorMap[y, x] != canvasBackColorMap[y, x])
                {
                    canvasBackColorMap[y, x] = frameBackColorMap[y, x];
                    tFlag = true;
                }
            }
            if (frameCharMap[y][x] != canvasCharMap[y][x])
            {
                canvasCharMap[y][x] = frameCharMap[y][x];
                tFlag = true;
            }

            if (tFlag)
            {
                refreshMap[y, x] = true;
                refreshMap[y, Size.X] = true;
            }
        }

        public void AddCanvas(Canvas canvas, int x, int y)
        {
            AddCanvas(canvas, x, y, 0, 0, canvas.Size.X, canvas.Size.Y);
        }

        public void AddCanvas(Canvas canvas, int x, int y,int fromX, int fromY, int fromWidth, int fromHeight)
        {

            Int2 otherSize = canvas.Size;


            if (x < 0)
                x = 0;
            int xStart = x;

            if (y < 0)
                y = 0;

            int otherX;
            int otherY = fromY;
            fromHeight = Arit.Clamp(fromHeight + fromY, 0, canvas.Size.Y);
            fromWidth = Arit.Clamp(fromWidth + fromX, 0, canvas.Size.X);
            while (y < Size.Y && otherY < fromHeight)
            {

                otherX = fromX;
                x = xStart;
                while (x < Size.X && otherX < fromWidth)
                {

                    frameFrontColorMap[y, x] = canvas.canvasFrontColorMap[otherY, otherX];
                    frameBackColorMap[y, x] = canvas.canvasBackColorMap[otherY, otherX];
                    frameCharMap[y][x] = canvas.canvasCharMap[otherY][otherX];

                    otherX++;
                    x++;
                }
                refreshMap[y, Size.X] = true;
                otherY++;
                y++;
            }

            canvasUpToDate = false;
        }

    }
}
