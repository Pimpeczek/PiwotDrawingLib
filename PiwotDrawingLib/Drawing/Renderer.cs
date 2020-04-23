using Pastel;
using PiwotToolsLib.PMath;
using System.Collections.Generic;

namespace PiwotDrawingLib.Drawing
{
    public static class Renderer
    {

        #region Variables
        private static Int2 maxWindowSize;
        private static Int2 windowSize;
        //private static Int2 canvasSize;

        static Canvas canvas;

        /// <summary>
        /// Title of the console window.
        /// </summary>
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
                while (nowDrawingFrame) { }
                windowSize = value;
                canvas.ResizeCanvas(new Int2(windowSize));
                SetupConsoleWindow();
            }
        }

        public static readonly string defFHex = "FFFFFF";
        public static readonly string defBHex = "000000";
        static readonly string defFHexTag = $"<cfFFFFFF>";
        static readonly string defBHexTag = $"<cb000000>";


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
                canvas.UseColor = value;
            }
        }

        private static List<UI.Containers.Container> containerRegistry;

        #endregion

        #region Setup
        static Renderer()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            WindowTitle = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            System.Console.CursorVisible = false;
            windowSize = new Int2(System.Console.WindowWidth, System.Console.WindowHeight);
            maxWindowSize = new Int2(System.Console.LargestWindowWidth, System.Console.LargestWindowHeight-1);
            canvas = new Canvas(windowSize);

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
            UseColor = true;


            containerRegistry = new List<UI.Containers.Container>();
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
                    for(int i = 0; i < containerRegistry.Count; i++)
                    {
                        containerRegistry[i].PrintOnCanvas(canvas);
                    }
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
                        canvas.DrawOnCanvas(dr.Text, dr.FID, dr.BID, dr.X, dr.Y);
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
            if (y >= canvas.Size.Y || y < 0 || x > canvas.Size.X || text.Length == 0)
            {
                return;
            }
            if (asyncDrawing)
            {
                drawingRequests.Enqueue(new DrawingRequest(text, fID, bID, x, y));
            }
            else
            {
                canvas.DrawOnCanvas(text, fID, bID, x, y);
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
            Int2 canvasSize = canvas.Size;
            string curBCol;
            string curFCol;
            string prevBCol;
            string prevFCol;
            string retStr;
            bool forceFull = false;
            canvas.ApplyNewFrame();
            if (System.Console.WindowWidth != windowSize.X || System.Console.WindowHeight != windowSize.Y)
            {
                SetupConsoleWindow();
                forceFull = true;
            }
            
            for (int y = 0; y < canvasSize.Y; y++)
            {
                if (canvas.refreshMap[y, canvasSize.X] || forceFull)
                {
                    startpos = -1;
                    endpos = -1;
                    retStr = "";

                    for (int x = 0; startpos < 0 && x < canvasSize.X; x++) 
                        if (canvas.refreshMap[y, x] || forceFull)
                            startpos = x;

                    if (startpos >= 0)
                    {
                        for (int x = canvasSize.X - 1; endpos < 0 && x >= 0 && x >= startpos; x--)
                            if (canvas.refreshMap[y, x] || forceFull)
                                endpos = x;

                        prevFCol = canvas.canvasFrontColorMap[y, startpos];
                        prevBCol = canvas.canvasBackColorMap[y, startpos];

                        curFCol = "";
                        curBCol = "";
                        strPos = startpos;
                        if (useColor)
                        {
                            for (int x = startpos; x <= endpos; x++)
                            {
                                if (x < endpos)
                                {
                                    curFCol = canvas.canvasFrontColorMap[y, x + 1];
                                    curBCol = canvas.canvasBackColorMap[y, x + 1];
                                }
                                if(curFCol == null || prevBCol==null)
                                {
                                }
                                else if (curFCol != prevFCol || prevBCol != curBCol || x == endpos)
                                {
                                    retStr += new string(canvas.canvasCharMap[y], strPos, x - strPos + 1).Pastel(prevFCol).PastelBg(prevBCol);

                                    strPos = x + 1;
                                    prevBCol = curBCol;
                                    prevFCol = curFCol;
                                }
                            }
                        }

                        else
                        {
                            retStr = new string(canvas.canvasCharMap[y], strPos, canvas.canvasCharMap[y].Length + 1 - strPos - endpos).Pastel(defFHex).PastelBg(defBHex);
                        }

                        while (nowWrittingRaw) { } 

                        RawPrint(retStr, startpos, y);
                        for (int i = startpos; i <= endpos; i++)
                            canvas.refreshMap[y, i] = false;
                        canvas.refreshMap[y, canvasSize.X] = false;
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

        /// <summary>Draws a given canvas on a given position.</summary>
        /// <para>For a string representation with '\n' character(s) Renderer will print all lines in a column.</para>
        /// <param name="printedCanvas">Canvas to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>>
        public static void Draw(Canvas printedCanvas, int x, int y)
        {
            canvas.AddCanvas(printedCanvas, x, y);
        }

        /// <summary>Draws a part of a given canvas on a given position.</summary>
        /// <para>For a string representation with '\n' character(s) Renderer will print all lines in a column.</para>
        /// <param name="printedCanvas">Canvas to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>>
        /// <param name="fromX">Horizontal distance from the top left corner of the printed canvas.</param>
        /// <param name="fromY">Vertical distance from the top left corner of the printed canvas.</param>
        /// <param name="width">The width of the drawn part of the given canvas.</param>
        /// <param name="height">The height of the drawn part of the given canvas.</param>
        public static void Draw(Canvas printedCanvas, int x, int y, int fromX, int fromY, int width, int height)
        {
            canvas.AddCanvas(printedCanvas, x, y, fromX, fromY, width, height);
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

        #region UI operations


        /// <summary>
        /// Registers a given container in a async printing registry to be printed every frame if the AsyncMode is turned on.
        /// </summary>
        /// <param name="container"></param>
        public static void RegisterContainer(UI.Containers.Container container)
        {
            if (containerRegistry.Contains(container))
                return;
            containerRegistry.Add(container);
            container.Register();
        }

        /// <summary>
        /// Unregisters a given container from the async printing registry.
        /// </summary>
        /// <param name="container"></param>
        public static void UnregisterContainer(UI.Containers.Container container)
        {
            if (!containerRegistry.Contains(container))
                return;
            containerRegistry.Remove(container);
            container.Unregister();
        }



        #endregion

    }
}
