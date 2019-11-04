using System;
using System.Collections.Concurrent;
using System.Threading;
using Pastel;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.Drawing
{
    static class Renderer
    {
        private static Int2 maxWindowSize;
        private static Int2 windowSize;
        private static Int2 canvasSize;
        public static Int2 WindowSize
        {
            get
            {
                return windowSize;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value < Int2.One || value > maxWindowSize)
                    throw new Exceptions.InvalidWindowSizeException();
                if (value == windowSize)
                    return;
                windowSize = value;
                ResizeCanvas(new Int2(windowSize.X, windowSize.Y));
                Console.SetWindowSize(windowSize.X, windowSize.Y+1);
            }
        }

        static string defFHex = "FFFFFF";
        static string defBHex = "000000";
        static string defFHexTag = $"<cfFFFFFF>";
        static string defBHexTag = $"<cb000000>";

        static int[,] frameFrontColorMap;
        static int[,] frameBackColorMap;
        static char[][] frameCharMap;

        static int[,] canvasFrontColorMap;
        static int[,] canvasBackColorMap;
        static char[][] canvasCharMap;

        static bool[,] refreshMap;
        static int colorPoint;

        static string[] colorDict;

        static int colorDictLength;

        static int frameLenght;
        public static int FrameLenght
        {
            get
            {
                return frameLenght;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                frameLenght = value;
            }
        }

        static Thread drawingThread;
        static Thread dequeuingThread;

        static bool nowWrittingRaw;
        static bool nowDrawingFrame;
        static readonly ConcurrentQueue<DrawingRequest> drawingRequests;
        static long time;

        static bool asyncDrawing;
        public static bool AsyncDrawing
        {
            get
            {
                return asyncDrawing;
            }
            set
            {
                if (asyncDrawing == value)
                    return;
                asyncDrawing = value;
                if(asyncDrawing)
                {
                    drawingThread = new Thread(PrintingLoop);
                    dequeuingThread = new Thread(DequeuingLoop);
                    drawingThread.Start();
                    dequeuingThread.Start();
                }
                else
                {
                    drawingThread.Abort();
                    dequeuingThread.Abort();
                }
            }
        }

        static bool useColor;
        public static bool UseColor
        {
            get
            {
                return useColor;
            }
            set
            {
                useColor = value;
                if(!useColor)
                {
                    colorDict[0] = defFHex;
                    colorDict[1] = defBHex;
                }
            }
        }
        

        static Renderer()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            windowSize = new Int2(Console.WindowWidth, Console.WindowHeight);
            maxWindowSize = new Int2(Console.LargestWindowWidth, Console.LargestWindowHeight-1);
            CreateCanvas();
            colorDictLength = 256;
            colorDict = new string[colorDictLength];
            colorDict[0] = defFHex;
            colorDict[1] = defBHex;
            colorPoint = 1;
            nowWrittingRaw = false;
            nowDrawingFrame = false;
            frameLenght = 30;
            drawingRequests = new ConcurrentQueue<DrawingRequest>();
            drawingThread = new Thread(PrintingLoop);
            dequeuingThread = new Thread(DequeuingLoop);
            time = 0;
            drawingThread.Start();
            dequeuingThread.Start();
            asyncDrawing = true;
            useColor = true;
        }

        private static void PrintingLoop()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            int sleepTime;
            while(true)
            {
                stopwatch.Restart();
                ForcePrint();
                sleepTime = frameLenght - (int)stopwatch.ElapsedMilliseconds;
                time += stopwatch.ElapsedMilliseconds;
                if (time >= 1000)
                {
                    time = 0;
                    GC.Collect();
                }
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
                 

            }
        }
        private static void DequeuingLoop()
        {
            
            while (true)
            {
                while (nowDrawingFrame) { }
                if (drawingRequests.TryDequeue(out DrawingRequest dr))
                {
                    DrawOnCanvas(dr.Text, dr.FID, dr.BID, dr.X, dr.Y);


                }
            }
        }

        static void DrawOnCanvas(string Text, int FID, int BID, int x, int y)
        {
            for (int i = 0; i < Text.Length && x < canvasSize.X; i++)
            {
                if (x >= 0)
                {
                    frameCharMap[y][x] = Text[i];

                    frameFrontColorMap[y, x] = FID;

                    frameBackColorMap[y, x] = BID;
                    
                }

                x++;

            }
            refreshMap[y, canvasSize.X] = true;
        }

        public static void ForcePrint()
        {
            nowDrawingFrame = true;
            int startpos;
            int endpos;
            int strPos;
            int curBCol;
            int curFCol;
            int prevBCol;
            int prevFCol;
            string retStr;
            ApplyNewFrame();

            for (int y = 0; y < canvasSize.Y; y++)
            {
                if (refreshMap[y, canvasSize.X])
                {
                    startpos = -1;
                    endpos = -1;
                    retStr = "";

                    for (int x = 0; startpos < 0 && x < canvasSize.X; x++) 
                        if (refreshMap[y, x])
                            startpos = x;

                    if (startpos >= 0)
                    {
                        for (int x = canvasSize.X - 1; endpos < 0 && x >= 0 && x >= startpos; x--)
                            if (refreshMap[y, x])
                                endpos = x;

                        prevFCol = canvasFrontColorMap[y, startpos];
                        prevBCol = canvasBackColorMap[y, startpos];
                        curFCol = -1;
                        curBCol = -1;
                        strPos = startpos;
                        for (int x = startpos; x <= endpos; x++)
                        {
                            if (x < endpos)
                            {
                                curFCol = canvasFrontColorMap[y, x + 1];
                                curBCol = canvasBackColorMap[y, x + 1];
                            }
                            if (curFCol != prevFCol || prevBCol != curBCol || x == endpos)
                            {
                                retStr += new string(canvasCharMap[y], strPos, x - strPos + 1).Pastel(colorDict[prevFCol]).PastelBg(colorDict[prevBCol]);

                                strPos = x + 1;
                                prevBCol = curBCol;
                                prevFCol = curFCol;
                            }
                        }

                        while (nowWrittingRaw) { } 

                        RawWrite(retStr, startpos, y);
                        for (int i = startpos; i <= endpos; i++)
                            refreshMap[y, i] = false;
                        refreshMap[y, canvasSize.X] = false;
                    }


                }
            }
            nowDrawingFrame = false;
        }
        public static void RawWrite(string text, int x, int y)
        {
            nowWrittingRaw = true;
            Console.SetCursorPosition(x, y);
            Console.Write(text);
            nowWrittingRaw = false;
        }


        /// <summary>Draws a given integer on a given position.</summary>
        /// <param name="integer">Integer to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(int integer, int x, int y)
        {
            Draw(integer.ToString(), defFHex, defBHex, x, y);
        }

        /// <summary>
        /// Draws a given integer on a given position.
        /// </summary>
        /// <param name="integer">Integer to be printed.</param>
        /// <param name="foregroundHex"></param>
        /// <param name="backgroundHex"></param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(int integer, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(integer.ToString(), foregroundHex, backgroundHex, x, y);
        }


        public static void DrawFormated(string text, int x, int y)
        {
            //RawWrite(text, x, y);
            text = text.Replace("</cf>", defFHexTag);
            text = text.Replace("</cb>", defBHexTag);
            string curFHex = defFHex;
            string curBHex = defBHex;
            int pos = text.IndexOf("<c");
            int prevPos = 0;
            bool isBackground;
            int xOffset = 0;
            while (pos >= 0)
            {
                if (text[pos + 9] != '>')
                {
                    continue;
                }

                if (text[pos + 2] == 'f')
                {
                    isBackground = false;

                }
                else if (text[pos + 2] == 'b')
                {
                    isBackground = true;
                }
                else
                {
                    throw new Exceptions.InvalidFormatException();
                }

                if (pos > prevPos)
                {

                    //retStr += str.Substring(prevPos, pos - prevPos).Pastel(curFHex).PastelBg(curBHex);
                    //Rendering.Renderer.Write(str.Substring(prevPos, str.Length - prevPos) + " ", 60, 1);
                    Draw(text.Substring(prevPos, pos - prevPos), curFHex, curBHex, x + xOffset, y);
                    xOffset += pos - prevPos;
                }

                if (isBackground)
                {
                    curBHex = text.Substring(pos + 3, 6);
                    TryAddColor(curBHex);
                }
                else
                {
                    curFHex = text.Substring(pos + 3, 6);
                    TryAddColor(curFHex);
                }

                prevPos = pos + 10;
                pos = text.IndexOf("<c", prevPos);
            }
            if (text.Length >= prevPos && pos != prevPos)
            {
                //retStr += str.Substring(prevPos, str.Length - prevPos).Pastel(curFHex).PastelBg(curBHex);
                //RawWrite(text.Substring(prevPos, text.Length - prevPos) + " ", 0, 49);
                //Console.ReadKey(true);
                Draw(text.Substring(prevPos, text.Length - prevPos), curFHex, curBHex, x + xOffset, y);
            }
        }

        public static void Draw(string text, string foregroundHex, string backgroundHex, int x, int y)
        {
            if (useColor)
            {
                Draw(text, TryAddColor(foregroundHex), TryAddColor(backgroundHex), x, y);
            }
            else
            {
                Draw(text, 0, 1, x, y);
            }
        }

        public static void Draw(string text, int x, int y)
        {
            Draw(text, 0, 1, x, y);
        }

        private static void Draw(string text, int fID, int bID, int x, int y)
        {
            if (y >= canvasSize.Y || y < 0 || x > canvasSize.X || text.Length == 0)
            {
                return;
            }
            if (asyncDrawing)
            {
                drawingRequests.Enqueue(new DrawingRequest(text, fID, bID, x, y));
            }
            else
            {
                DrawOnCanvas(text, fID, bID, x, y);
            }
            
        }

        

        public static void ApplyNewFrame()
        {
            for (int y = 0; y < canvasSize.Y; y++)
            {
                if (refreshMap[y, canvasSize.X])
                {
                    refreshMap[y, canvasSize.X] = false;
                    for (int x = 0; x < canvasSize.X; x++)
                    {
                        CheckOnePixel(x, y);
                    }
                }
            }
        }

        private static void CheckOnePixel(int x, int y)
        {
            bool tFlag = false;
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

            if (frameCharMap[y][x] != canvasCharMap[y][x])
            {
                canvasCharMap[y][x] = frameCharMap[y][x];
                tFlag = true;
            }

            if (tFlag)
            {
                refreshMap[y, x] = true;
                refreshMap[y, canvasSize.X] = true;
            }
        }

        private static int TryAddColor(string hex)
        {
            for (int i = 0; i < colorDict.Length; i++)
            {
                if (colorDict[i] == hex)
                {
                    return i;
                }
            }
            colorPoint++;
            if (colorPoint >= colorDict.Length)
                colorPoint = 2;
            colorDict[colorPoint] = hex;

            return colorPoint;
        }

        private static void CreateCanvas()
        {
            canvasSize = new Int2(windowSize.X, windowSize.Y);
            frameFrontColorMap = new int[canvasSize.Y, canvasSize.X];
            frameBackColorMap = new int[canvasSize.Y, canvasSize.X];
            frameCharMap = new char[canvasSize.Y][];

            canvasFrontColorMap = new int[canvasSize.Y, canvasSize.X];
            canvasBackColorMap = new int[canvasSize.Y, canvasSize.X];
            canvasCharMap = new char[canvasSize.Y][];

            refreshMap = new bool[canvasSize.Y, canvasSize.X + 1];

            for (int i = 0; i < canvasSize.Y; i++)
            {
                frameCharMap[i] = new char[canvasSize.X];

                canvasCharMap[i] = new char[canvasSize.X];
                refreshMap[i, canvasSize.X] = false;
                for (int j = 0; j < canvasSize.X; j++)
                {
                    frameCharMap[i][j] = ' ';
                    frameFrontColorMap[i, j] = 0;
                    frameBackColorMap[i, j] = 1;

                    canvasCharMap[i][j] = ' ';
                    canvasFrontColorMap[i, j] = 0;
                    canvasBackColorMap[i, j] = 1;

                    refreshMap[i, j] = false;
                }
            } 
        }

        private static void ResizeCanvas(Int2 newSize)
        {
            int[,] newFrameFrontColorMap;
            int[,] newFrameBackColorMap;
            char[][] newFrameCharMap;

            int[,] newCanvasFrontColorMap;
            int[,] newCanvasBackColorMap;
            char[][] newCanvasCharMap;

            bool[,] newRefreshMap;
            newFrameFrontColorMap = new int[newSize.Y, newSize.X];
            newFrameBackColorMap = new int[newSize.Y, newSize.X];
            newFrameCharMap = new char[newSize.Y][];

            newCanvasFrontColorMap = new int[newSize.Y, newSize.X];
            newCanvasBackColorMap = new int[newSize.Y, newSize.X];
            newCanvasCharMap = new char[newSize.Y][];

            newRefreshMap = new bool[newSize.Y, newSize.X + 1];

            for (int i = 0; i < newSize.Y && i < canvasSize.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = 0; j < newSize.X && j < canvasSize.X; j++)
                {
                    newFrameCharMap[i][j] = frameCharMap[i][j];
                    newFrameFrontColorMap[i, j] = frameFrontColorMap[i, j];
                    newFrameBackColorMap[i, j] = frameBackColorMap[i, j];

                    newCanvasCharMap[i][j] = canvasCharMap[i][j];
                    newCanvasFrontColorMap[i, j] = canvasFrontColorMap[i, j];
                    newCanvasBackColorMap[i, j] = canvasBackColorMap[i, j];

                    newRefreshMap[i, j] = refreshMap[i, j];
                }
            }

            for (int i = canvasSize.Y; i < newSize.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = canvasSize.X; j < newSize.X; j++)
                {
                    newFrameCharMap[i][j] = ' ';
                    newFrameFrontColorMap[i, j] = 0;
                    newFrameBackColorMap[i, j] = 1;

                    newCanvasCharMap[i][j] = ' ';
                    newCanvasFrontColorMap[i, j] = 0;
                    newCanvasBackColorMap[i, j] = 1;

                    newRefreshMap[i, j] = false;
                }
            }
            canvasSize = new Int2(newSize);
            frameFrontColorMap = newFrameFrontColorMap;
            frameBackColorMap = newFrameBackColorMap;
            frameCharMap = newFrameCharMap;

            canvasFrontColorMap = newCanvasFrontColorMap;
            canvasBackColorMap = newCanvasBackColorMap;
            canvasCharMap = newCanvasCharMap;

            refreshMap = newRefreshMap;
        }


        #region Pastel

        /// <summary>
        /// Unwraps pastel wrapped string.
        /// </summary>
        /// <param name="str">Strinh to be unwrapped</param>
        /// <returns></returns>
        public static string UnwrapPastel(string str)
        {
            int wrapps = PiwotToolsLib.Data.Stringer.CountChars(str, (char)27) / 2;
            int maxMPos = 21 * wrapps + 1;
            int mPos = 0;
            for (int i = 11; i <= maxMPos; i++)
            {
                if (str[i] == 'm')
                {
                    mPos++;
                    if (mPos == wrapps)
                    {
                        mPos = i + 1;
                        i = maxMPos + 1;
                    }
                }
            }
            return str.Substring(mPos, str.Length - mPos - 4 * wrapps);
        }
        #endregion

    }
}
