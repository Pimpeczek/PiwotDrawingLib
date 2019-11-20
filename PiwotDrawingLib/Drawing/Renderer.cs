using Pastel;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.Drawing
{
    public static class Renderer
    {

        #region Variables
        private static Int2 maxWindowSize;
        private static Int2 windowSize;
        private static Int2 canvasSize;


        public static string WindowTitle
        {
            get
            {
                return System.Console.Title;
            }
            set
            {
                System.Console.Title = value;
            }
        }
        /// <summary>
        /// Size of the console window. 
        /// </summary>
        public static Int2 WindowSize
        {
            get
            {
                return windowSize;
            }
            set
            {
                if (value == null)
                    throw new System.ArgumentNullException();
                if (value < Int2.One || value > maxWindowSize)
                    throw new Exceptions.InvalidWindowSizeException();
                if (value == windowSize)
                    return;
                windowSize = value;
                ResizeCanvas(new Int2(windowSize.X, windowSize.Y));
                SetupConsoleWindow();
            }
        }

        static readonly string defFHex = "FFFFFF";
        static readonly string defBHex = "000000";
        static readonly string defFHexTag = $"<cfFFFFFF>";
        static readonly string defBHexTag = $"<cb000000>";

        static string[,] frameFrontColorMap;
        static string[,] frameBackColorMap;
        static char[][] frameCharMap;

        static string[,] canvasFrontColorMap;
        static string[,] canvasBackColorMap;
        static char[][] canvasCharMap;

        static bool[,] refreshMap;


        static int frameLenght;

        /// <summary>
        /// Amount of time in miliseconds the renderer waits until next screen refresh.
        /// </summary>
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
                    throw new System.ArgumentOutOfRangeException();
                }
                frameLenght = value;
            }
        }

        /// <summary>
        /// Thread responsible for asynchronous printing.
        /// </summary>
        static System.Threading.Thread drawingThread;
        /// <summary>
        /// Thread responsible for asynchronous dequeuing of DrawingRequests. 
        /// </summary>
        static System.Threading.Thread dequeuingThread;

        /// <summary>
        /// Flag used to prevent simultaneous printing.
        /// </summary>
        static bool nowWrittingRaw;
        /// <summary>
        /// Flag used to prevent dequeing while printing canvas.
        /// </summary>
        static bool nowDrawingFrame;

        static readonly System.Collections.Concurrent.ConcurrentQueue<DrawingRequest> drawingRequests;
        static long time;

        static bool asyncDrawing;
        /// <summary>
        /// While set to true: the Renderer will try to refresh canvas every FrameLenght miliseconds.  
        /// <para>While set to false: the ForcePrint method must be invoked by hand in order to refresh canvas.</para>
        /// </summary>
        public static bool AsyncMode
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
                    
                }
                else
                {
                    drawingThread.Abort();
                    dequeuingThread.Abort();
                }
            }
        }

        static bool useColor;

        /// <summary>
        /// <para>If set to true the Renderer will use Pastel library to display colored characters.</para>
        /// <para>If set to false the Renderer will use only default colors to display characters, but will work much faster.</para>
        /// <para>Refresing while canvas might be advised while changing this flag after something was already printed.</para>
        /// </summary>
        public static bool UseColor
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
        #endregion

        #region Setup
        static Renderer()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            WindowTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            System.Console.CursorVisible = false;
            windowSize = new Int2(System.Console.WindowWidth, System.Console.WindowHeight);
            maxWindowSize = new Int2(System.Console.LargestWindowWidth, System.Console.LargestWindowHeight-1);
            CreateCanvas();
           
            nowWrittingRaw = false;
            nowDrawingFrame = false;
            frameLenght = 30;
            drawingRequests = new System.Collections.Concurrent.ConcurrentQueue<DrawingRequest>();
            drawingThread = new System.Threading.Thread(PrintingLoop);
            dequeuingThread = new System.Threading.Thread(DequeuingLoop);
            time = 0;
            drawingThread.Start();
            dequeuingThread.Start();
            asyncDrawing = true;
            useColor = true;
            
        }

        private static void CreateCanvas()
        {
            canvasSize = new Int2(windowSize.X, windowSize.Y);
            frameFrontColorMap = new string[canvasSize.Y, canvasSize.X];
            frameBackColorMap = new string[canvasSize.Y, canvasSize.X];
            frameCharMap = new char[canvasSize.Y][];

            canvasFrontColorMap = new string[canvasSize.Y, canvasSize.X];
            canvasBackColorMap = new string[canvasSize.Y, canvasSize.X];
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
                    frameFrontColorMap[i, j] = defFHex;
                    frameBackColorMap[i, j] = defBHex;

                    canvasCharMap[i][j] = ' ';
                    canvasFrontColorMap[i, j] = defFHex;
                    canvasBackColorMap[i, j] = defBHex;

                    refreshMap[i, j] = false;
                }
            }
        }

        private static void ResizeCanvas(Int2 newSize)
        {
            while (nowDrawingFrame) { }
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
                for (int j = canvasSize.X; j < newSize.X; j++)
                {
                    newFrameCharMap[i][j] = ' ';
                    newFrameFrontColorMap[i, j] = defFHex;
                    newFrameBackColorMap[i, j] = defBHex;

                    newCanvasCharMap[i][j] = ' ';
                    newCanvasFrontColorMap[i, j] = defFHex;
                    newCanvasBackColorMap[i, j] = defBHex;
                    newRefreshMap[i, j] = false;
                }
            }

            for (int i = canvasSize.Y; i < newSize.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = 0; j < newSize.X; j++)
                {
                    newFrameCharMap[i][j] = ' ';
                    newFrameFrontColorMap[i, j] = defFHex;
                    newFrameBackColorMap[i, j] = defBHex;

                    newCanvasCharMap[i][j] = ' ';
                    newCanvasFrontColorMap[i, j] = defFHex;
                    newCanvasBackColorMap[i, j] = defBHex;
                    newRefreshMap[i, j] = false;
                }
            }
            canvasSize = new Int2(newSize);
            frameFrontColorMap = (string[,])newFrameFrontColorMap.Clone();
            frameBackColorMap = (string[,])newFrameBackColorMap.Clone();
            frameCharMap = newFrameCharMap;

            canvasFrontColorMap = (string[,])newCanvasFrontColorMap.Clone();
            canvasBackColorMap = (string[,])newCanvasBackColorMap.Clone();
            canvasCharMap = newCanvasCharMap;

            refreshMap = newRefreshMap;
        }

        private static void SetupConsoleWindow()
        {
            try
            {
                System.Console.SetWindowPosition(0, 0);
                System.Console.SetWindowSize(windowSize.X, windowSize.Y);
                System.Console.SetBufferSize(windowSize.X, windowSize.Y);
                System.Console.SetWindowPosition(0, 0);
                System.Console.CursorVisible = false;
            }
            catch
            {
                System.Threading.Thread.Sleep(100);
                SetupConsoleWindow();
            }

        }

        #endregion

        #region Asynchronous loops and threading
        private static void PrintingLoop()
        {
            try
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                int sleepTime;
                while (true)
                {
                    stopwatch.Restart();
                    ForcePrint();
                    sleepTime = frameLenght - (int)stopwatch.ElapsedMilliseconds;
                    time += stopwatch.ElapsedMilliseconds;
                    if (time >= 1000)
                    {
                        time = 0;
                        //System.GC.Collect();
                    }
                    if (sleepTime > 0)
                        System.Threading.Thread.Sleep(sleepTime);
                }
            }
            catch(System.Threading.ThreadAbortException)
            {
                
            }
        }

        private static void DequeuingLoop()
        {
            try
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
            catch (System.Threading.ThreadAbortException)
            {

            }
        }

        /// <summary>
        /// Aborts printing and queue threads.
        /// </summary>
        public static void StopThreads()
        {
            drawingThread.Abort();
            dequeuingThread.Abort();
        }

        /// <summary>
        /// Restarts printing and queue threads if AsyncMode is enabled and threads are not already running.
        /// </summary>
        public static void RestartThreads()
        {
            
            if (!asyncDrawing)
                return;
            if (drawingThread.ThreadState != System.Threading.ThreadState.Running)
            {
                drawingThread = new System.Threading.Thread(PrintingLoop);
                drawingThread.Start();
            }
            if (dequeuingThread.ThreadState != System.Threading.ThreadState.Running)
            {
                dequeuingThread = new System.Threading.Thread(DequeuingLoop);
                dequeuingThread.Start();
            }
            
        }

        #endregion

        #region Canvas Operations
        static void DrawOnCanvas(string Text, string FID, string BID, int x, int y)
        {

            for (int i = 0; i < Text.Length && x < canvasSize.X; i++)
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
            refreshMap[y, canvasSize.X] = true;
        }

        /// <summary>
        /// Draws current frame on the canvas and updates difference map.
        /// </summary>
        public static void ApplyNewFrame()
        {
            for (int y = 0; y < canvasSize.Y; y++)
            {
                if (refreshMap[y, canvasSize.X])
                {
                    refreshMap[y, canvasSize.X] = false;
                    for (int x = 0; x < canvasSize.X; x++)
                    {
                        UpdateOneCell(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the specified frame cell is different from canvas cell and if so updates the canvas as well as refresh map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void UpdateOneCell(int x, int y)
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
                refreshMap[y, canvasSize.X] = true;
            }
        }
        /// <summary>
        /// Creates and enqueues new drawing request.
        /// </summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="fID">Id of the foreground color.</param>
        /// <param name="bID">Id of the background color.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        private static void DrawR(string text, string fID, string bID, int x, int y)
        {
            if (text.Contains("\n"))
            {
                string[] split = text.Split('\n');
                for (int i = 0; i < split.Length; i++)
                {
                    DrawR(split[i], fID, bID, x, y + i);
                }
                return;
            }
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

        #endregion

        #region Printing
        /// <summary>
        /// Prints any changes between current frame and the previous one. Should not be invoked while using AsyncDrawing.
        /// </summary>
        public static void ForcePrint()
        {
            nowDrawingFrame = true;
            int startpos;
            int endpos;
            int strPos;
            string curBCol = defBHex;
            string curFCol = defBHex;
            string prevBCol = defBHex;
            string prevFCol = defBHex;
            string retStr;
            bool forceFull = false;
            ApplyNewFrame();
            if (System.Console.WindowWidth != windowSize.X || System.Console.WindowHeight != windowSize.Y)
            {
                SetupConsoleWindow();
                forceFull = true;
            }
            
            for (int y = 0; y < canvasSize.Y; y++)
            {
                if (refreshMap[y, canvasSize.X] || forceFull)
                {
                    startpos = -1;
                    endpos = -1;
                    retStr = "";

                    for (int x = 0; startpos < 0 && x < canvasSize.X; x++) 
                        if (refreshMap[y, x] || forceFull)
                            startpos = x;

                    if (startpos >= 0)
                    {
                        for (int x = canvasSize.X - 1; endpos < 0 && x >= 0 && x >= startpos; x--)
                            if (refreshMap[y, x] || forceFull)
                                endpos = x;

                        prevFCol = canvasFrontColorMap[y, startpos];
                        prevBCol = canvasBackColorMap[y, startpos];

                        curFCol = "";
                        curBCol = "";
                        strPos = startpos;
                        if (useColor)
                        {
                            for (int x = startpos; x <= endpos; x++)
                            {
                                if (x < endpos)
                                {
                                    curFCol = canvasFrontColorMap[y, x + 1];
                                    curBCol = canvasBackColorMap[y, x + 1];
                                }
                                if(curFCol == null || prevBCol==null)
                                {
                                    System.Console.WriteLine($"{x}, {y}");
                                    System.Console.ReadKey(true);
                                }
                                else if (curFCol != prevFCol || prevBCol != curBCol || x == endpos)
                                {
                                    retStr += new string(canvasCharMap[y], strPos, x - strPos + 1).Pastel(prevFCol).PastelBg(prevBCol);

                                    strPos = x + 1;
                                    prevBCol = curBCol;
                                    prevFCol = curFCol;
                                }
                            }
                        }

                        else
                        {
                            retStr = new string(canvasCharMap[y], strPos, canvasCharMap[y].Length + 1 - strPos - endpos).Pastel(defFHex).PastelBg(defBHex);
                        }

                        while (nowWrittingRaw) { } 

                        RawPrint(retStr, startpos, y);
                        for (int i = startpos; i <= endpos; i++)
                            refreshMap[y, i] = false;
                        refreshMap[y, canvasSize.X] = false;
                    }


                }
            }
            nowDrawingFrame = false;
        }

        /// <summary>
        /// Allows to print raw string on a given position. This method will not affect the canvas, so the Renderer will not refresh cells this method printed over.
        /// </summary>
        /// <param name="text">The string to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void RawPrint(string text, int x, int y)
        {
            nowWrittingRaw = true;
            
            try
            {
                System.Console.SetCursorPosition(x, y);
                System.Console.Write(text);
            }
            catch
            {
                
            }
            nowWrittingRaw = false;
        }

        #endregion

        #region Public drawing methods
        /// <summary>Draws a given string on a given position. Using default colors.
        /// <para>For a string with '\n' character(s) Renderer will print all lines in a column.</para>
        /// </summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(string text, int x, int y)
        {
            DrawR(text, defFHex, defBHex, x, y);
        }
        /// <summary>
        /// Draws a given string on a given position with given foreground and background color.
        /// <para>For a string with '\n' character(s) Renderer will print all lines in a column.</para>
        /// </summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(string text, string foregroundHex, string backgroundHex, int x, int y)
        {
            if (useColor)
            {
                DrawR(text, foregroundHex, backgroundHex, x, y);
            }
            else
            {
                DrawR(text, defFHex, defBHex, x, y);
            }
        }

        /// <summary>Draws a given integer on a given position. Using default colors.</summary>
        /// <param name="value">Integer to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(int value, int x, int y)
        {
            Draw(value.ToString(), defFHex, defBHex, x, y);
        }
        /// <summary>
        /// Draws a given integer on a given position with given foreground and background color.
        /// </summary>
        /// <param name="value">Integer to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(int value, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(value.ToString(), foregroundHex, backgroundHex, x, y);
        }

        /// <summary>
        /// Draws a given float on a given position with given foreground and background color.
        /// </summary>
        /// <param name="value">Float to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(float value, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(value.ToString(), foregroundHex, backgroundHex, x, y);
        }
        /// <summary>Draws a given float on a given position. Using default colors.</summary>
        /// <param name="value">Float to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(float value, int x, int y)
        {
            Draw(value.ToString(), defFHex, defBHex, x, y);
        }

        /// <summary>
        /// Draws a given double on a given position with given foreground and background color.
        /// </summary>
        /// <param name="value">Double to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(double value, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(value.ToString(), foregroundHex, backgroundHex, x, y);
        }
        /// <summary>Draws a given double on a given position. Using default colors.</summary>
        /// <param name="value">Double to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(double value, int x, int y)
        {
            Draw(value.ToString(), defFHex, defBHex, x, y);
        }

        /// <summary>
        /// Draws a given long on a given position with given foreground and background color.
        /// </summary>
        /// <param name="value">Long to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(long value, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(value.ToString(), foregroundHex, backgroundHex, x, y);
        }
        /// <summary>Draws a given long on a given position. Using default colors.</summary>
        /// <param name="value">Long to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(long value, int x, int y)
        {
            Draw(value.ToString(), defFHex, defBHex, x, y);
        }

        /// <summary>Draws a given boolean on a given position. Using default colors.</summary>
        /// <param name="value">Boolean to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(bool value, int x, int y)
        {
            Draw(value.ToString(), defFHex, defBHex, x, y);
        }
        /// <summary>
        /// Draws a given boolean on a given position with given foreground and background color.
        /// </summary>
        /// <param name="value">Boolean to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(bool value, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(value.ToString(), foregroundHex, backgroundHex, x, y);
        }

        /// <summary>
        /// Draws a given object on a given position with given foreground and background color.
        /// <para>For a string representation with '\n' character(s) Renderer will print all lines in a column.</para>
        /// </summary>
        /// <param name="value">Object to be printed.</param>
        /// <param name="foregroundHex">Foreground color in hex notation. E.g. "FFFFFF"</param>
        /// <param name="backgroundHex">Background color in hex notation. E.g. "000000"</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(object value, string foregroundHex, string backgroundHex, int x, int y)
        {
            Draw(value.ToString(), foregroundHex, backgroundHex, x, y);
        }
        /// <summary>Draws a given object on a given position. Using default colors.</summary>
        /// <para>For a string representation with '\n' character(s) Renderer will print all lines in a column.</para>
        /// <param name="value">Object to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Draw(object value, int x, int y)
        {
            Draw(value.ToString(), defFHex, defBHex, x, y);
        }



        /// <summary>
        /// Draws a given string on a given position using format tags to set desired colors for printing.
        /// </summary>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        /// <param name="text">The formated string to be printed.</param>
        public static void DrawFormated(string text, int x, int y)
        {

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
                    //TryAddColor(curBHex);
                }
                else
                {
                    curFHex = text.Substring(pos + 3, 6);
                    //TryAddColor(curFHex);
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


        #endregion

        #region Colors

        public static string StripFormating(string formattedString)
        {
            return System.Text.RegularExpressions.Regex.Replace(formattedString,
                @"(\<c[fb][a-fA-F0-9]{6}\>)|(\</c[fb]\>)",
                @"");
        }

        #endregion

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
