using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Pastel;
using System.Drawing;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.Drawing
{

    public static class Renderer
    {

        #region Variables
        public enum HorizontalTextAlignment { left, middle, right }
        public enum VerticalTextAlignment { upper, middle, lower }

        /// <summary>Number of debug lines.</summary>
        static int debugLines = 0;

        /// <summary>Size of the console window.</summary>
        static Int2 windowSize = new Int2(50, 20);

        /// <summary>Size of the console window.</summary>
        public static Int2 WindowSize
        {
            get { return windowSize; }
            set
            {
                if (windowSize == value)
                    return;
                if (value.X < 0 || value.Y < 0)
                {
                    throw new ArgumentException("Width and height of console window must be greater than 0", "windowSize");
                }

                windowSize = value;
                if (debugMode)
                {
                    SetupDebugMode();
                }
                Console.SetWindowSize(windowSize.X, windowSize.Y);
                canvas.Resize(windowSize);
            }
        }
        static bool asyncMode;
        static Thread drawingThread;
        static Stopwatch stopwatch;
        static Queue requests = Queue.Synchronized(new Queue());
        static int requestPointer;
        static bool visableCursor;

        /// <summary>Enables or disables console cursor.</summary>
        public static bool VisableCursor
        {
            get { return visableCursor; }
            set
            {
                if (value == visableCursor)
                    return;
                Console.CursorVisible = value;
                visableCursor = value;
            }
        }
        static bool debugMode;

        /// <summary>Enables or disables debug drawing mode.</summary>
        public static bool DebugMode
        {
            get { return debugMode; }
            set
            {
                if (value == debugMode)
                    return;
                debugMode = value;
                if (debugMode)
                {
                    SetupDebugMode();
                }

                Console.SetWindowSize(windowSize.X, windowSize.Y);
            }
        }
        static long frame = 0;
        static long requestCount = 0;
        static int lastSleepTime = 0;
        static int droppedRequests = 0;

        /// <summary>Enables or disables asynchronous drawing mode.</summary>
        public static bool AsyncMode
        {
            get { return asyncMode; }
            set
            {
                if (asyncMode == value)
                    return;
                asyncMode = value;
                if (asyncMode)
                {
                    //requests = new Queue<RenderRequest>();
                    drawingThread = new Thread(AsyncDrawingLoop);
                    drawingThread.Start();
                }
                else
                {

                }
            }
        }
        static int asyncFrameLenght = 30;

        /// <summary>The interval in milliseconds between each frame render.</summary>
        public static int AsyncFrameLenght
        {
            get { return asyncFrameLenght; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("AsyncFrameLenght cannot be lower than 1", "asyncFrameLenght");
                }
                asyncFrameLenght = value;
            }
        }

        /// <summary>Enables or disables use of Pastel coloring. If enabled even pastel wrapped strings will render in the default console color.</summary>
        public static bool UsePastel { get; set; } = true;

        static int maxRequestCount = 8192;

        /// <summary>Number of request in the rendering queue required to pause the rendering.</summary>
        public static int MaxRequestCount
        {
            get { return maxRequestCount; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("MaxRequestCount cannot be lower than 1", "asyncFrameLenght");
                }
                maxRequestCount = value;
            }
        }

        static int emptyCheckingInterval = 16;

        /// <summary>The period of a loop checking if the rendering queue is empty.</summary>
        public static int EmptyCheckingInterval
        {
            get { return emptyCheckingInterval; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("PauseOnFullTime cannot be lower than 0", "pauseOnFullTime");
                }
                emptyCheckingInterval = value;
            }
        }

        static Canvas canvas;

        static List<Requests.TimedTextRequest> timedTextRequests;

        #endregion

        #region Setup
        static Renderer()
        {
            Console.OutputEncoding = Encoding.UTF8;
            timedTextRequests = new List<Requests.TimedTextRequest>();
            Console.CursorVisible = false;
            AsyncMode = true;
            requestPointer = 0;
            canvas = new Canvas(windowSize, 256);
        }


        /// <summary>Adjusts window height to compensate for the debug bar.</summary>
        static void SetupDebugMode()
        {
            debugLines = debugMode ? 1 : 0;
            windowSize.Y += debugLines;
        }

        #endregion

        #region Async Drawing
        /// <summary>The debug version of AsyncWritting. Will work slower than the normal version.</summary>
        static void AsyncDrawingDebug()
        {
            int sleepTime, elapsedTime, queleLen = 0, charCount = 0;
            
            stopwatch.Restart();
            canvas.Print();
            Requests.RenderRequest tRequest;
            while (requests.Count > 0)
            {
                queleLen++;
                tRequest = (Requests.RenderRequest)requests.Dequeue();
                charCount += tRequest.RawText.Length;
                Draw(tRequest);


            }
            requests.Clear();
            stopwatch.Stop();
            elapsedTime = (int)stopwatch.ElapsedMilliseconds;
            sleepTime = asyncFrameLenght - elapsedTime - Arit.Clamp(lastSleepTime, int.MinValue, 0);

            if (true)
            {
                string debugString = "";
                debugString += $"Frame len: {$"{asyncFrameLenght}".PadLeft(5)}, ";
                debugString += $"Elapsed: {$"{elapsedTime}".PadLeft(5)}, ";
                debugString += $"Sleep: {$"{sleepTime}".PadLeft(5)}, ";
                debugString += $"Queue len: {$"{queleLen}".PadLeft(5)}, ";
                debugString += $"Chars: {$"{charCount}".PadLeft(8)}, ";
                debugString += $"Requests: {$"{requestCount}".PadLeft(8)}, ";
                debugString += $"Hist ID: {$"{requestPointer}".PadLeft(4)}, ";
                debugString += $"Dropped: {$"{droppedRequests}".PadLeft(2)}";

                SyncDraw(debugString.PadRight(windowSize.X).Pastel(Color.DarkBlue).PastelBg(Color.LightGray), 0, 0);
            }
            SyncDraw($"Frame: {$"{frame}".PadLeft(8)}".Pastel(Color.DarkViolet).PastelBg(Color.LightGray), windowSize.X - 16, 0);
            lastSleepTime = sleepTime;
            AgeTimedTextRequests(elapsedTime + sleepTime);

            Thread.Sleep(Arit.Clamp(sleepTime, 0, asyncFrameLenght));
        }


        /// <summary>Asynchronously draws every request in a given drawing frame.</summary>
        static void AsyncDrawing()
        {
            stopwatch.Restart();
            while (requests.Count > 0)
            {
                Draw((Requests.RenderRequest)requests.Dequeue());
            }
            AgeTimedTextRequests((int)stopwatch.ElapsedMilliseconds);
            Thread.Sleep(Arit.Clamp(asyncFrameLenght - (int)stopwatch.ElapsedMilliseconds, 0, asyncFrameLenght));
        }


        /// <summary>Function containing a loop used for asynchronous drawing of requests.</summary>
        static void AsyncDrawingLoop()
        {
            frame = 0;
            stopwatch = new Stopwatch();

            try
            {
                while (asyncMode)
                {
                    Thread.Sleep(asyncFrameLenght);
                    if (debugMode)
                    {
                        AsyncDrawingDebug();
                    }
                    else
                    {
                        AsyncDrawing();
                    }
                    frame++;
                }
            }
            catch (ThreadAbortException e)
            {

            }
        }

        /// <summary>
        /// Aborts async thread safely.
        /// </summary>
        public static void AbortAsyncThread()
        {
            if (requests != null)
            {
                requests.Clear();
                requests = null;
            }
            if (drawingThread.IsAlive)
            {
                drawingThread.Abort();
            }
        }
        #endregion

        #region Sync Drawing
        /// <summary>Interprets and prints a render request.</summary>
        /// <param name="request">The request to be printed.</param>
        static void Draw(Requests.RenderRequest request)
        {
            requestCount++;
            SyncDraw(UsePastel ? request.Text : request.RawText, request.X, request.Y + debugLines);
        }

        /// <summary>Draws a given string on a given position.</summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        static void SyncDraw(string text, int x, int y)
        {
            try
            {
                Console.SetCursorPosition(x, y);
                Console.Write(text);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.SetWindowSize(windowSize.X, windowSize.Y);
                if (!visableCursor)
                    Console.CursorVisible = false;
            }
        }
        #endregion

        #region Writting

        /// <summary>Draws a given string on a given position. If AsyncMode is enabled it will add a render request to the queue.</summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="position">Horizontal(X) and vertical(Y) distance from the top left corner.</param>>
        public static void Write(string text, Int2 position)
        {
            Write(text, position.X, position.Y);
        }

        /// <summary>Draws a given string on a given position. If AsyncMode is enabled it will add a render request to the queue.</summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Write(string text, int x, int y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            if (AsyncMode)
            {
                //If queue is full stops the main thread until the queue is empty.
                //if (requests.Count >= maxRequestCount)
                //while (requests.Count > 0) Thread.Sleep(emptyCheckingInterval);

                //requests.Enqueue(new Requests.RenderRequest(text, x, y));
                canvas.Draw(text, x, y);
                return;
            }
            SyncDraw(text, x, y);
        }
        /// <summary>Draws a given string on a given position. Ommits rendering queue if AsyncMode is enabled. Draws from thread it was called.</summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="position">Horizontal(X) and vertical(Y) distance from the top left corner.</param>>
        public static void SyncWrite(string text, Int2 position)
        {
            SyncWrite(text, position.X, position.Y);
        }

        /// <summary>Draws a given string on a given position. Ommits rendering queue if AsyncMode is enabled. Draws from thread it was called.</summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void SyncWrite(string text, int x, int y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            Console.SetCursorPosition(x, y);
            Console.Write(text);
        }

        /// <summary>Draws a given integer on a given position.</summary>
        /// <param name="integer">Integer to be printed.</param>
        /// <param name="position">Horizontal(X) and vertical(Y) distance from the top left corner.</param>>
        public static void Write(int integer, Int2 position)
        {
            Write(integer.ToString(), position.X, position.Y);
        }
        /// <summary>Draws a given integer on a given position.</summary>
        /// <param name="integer">Integer to be printed.</param>
        /// <param name="x">Horizontal distance from the top left corner.</param>
        /// <param name="y">Vertical distance from the top left corner.</param>
        public static void Write(int integer, int x, int y)
        {
            Write(integer.ToString(), x, y);
        }
        #endregion

        #region Timed Text
        /// <summary>
        /// Draws given string on the screen and esases it after given time using spaces.
        /// </summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="timeOnScreen">Time in milliseconds after which text will dissapear.</param>
        /// <param name="position">Horizontal(X) and vertical(Y) distance from the top left corner.</param>
        public static Requests.TimedTextRequest AddDissapearingText(string text, int timeOnScreen, Int2 position)
        {
            return AddDissapearingText(text, "", timeOnScreen, position);
        }
        /// <summary>
        /// Draws given string on the screen and esases it after given time using given string.
        /// </summary>
        /// <param name="text">String to be printed.</param>
        /// <param name="clearingString">String to be used for drawing on the oryginal text.</param>
        /// <param name="timeOnScreen">Time in milliseconds after which text will dissapear.</param>
        /// <param name="position">Horizontal(X) and vertical(Y) distance from the top left corner.</param>
        /// <returns></returns>
        public static Requests.TimedTextRequest AddDissapearingText(string text, string clearingString, int timeOnScreen, Int2 position)
        {
            Requests.TimedTextRequest ttr = new Requests.TimedTextRequest(text, clearingString, timeOnScreen, position.X, position.Y);
            timedTextRequests.Add(ttr);
            Write(text, position);
            return ttr;
        }

        /// <summary>
        /// Erases given TimedTextRequest using its erase string.
        /// </summary>
        /// <param name="ttr">TimedTextRequest to be erased.</param>
        public static void EraseTimedText(Requests.TimedTextRequest ttr)
        {
            Write(ttr.ClearingString, ttr.X, ttr.Y);
            timedTextRequests.Remove(ttr);
        }

        /// <summary>
        /// Ages all active timed text requests.
        /// </summary>
        /// <param name="elapsed">Time in milliseconds passed between frames.</param>
        static void AgeTimedTextRequests(int elapsed)
        {
            for (int i = 0; i < timedTextRequests.Count; i++)
            {
                timedTextRequests[i].Age(elapsed);
            }
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