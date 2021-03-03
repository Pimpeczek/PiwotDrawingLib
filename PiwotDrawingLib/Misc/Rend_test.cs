using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiwotDrawingLib.Drawing;
namespace PiwotDrawingLib.Misc
{
    class Renderer
    {
        protected static string defFHex = "FFFFFF";
        protected static string defBHex = "000000";
        protected static string defFHexTag = $"<cf{defFHex}>";
        protected static string defBHexTag = $"<cb{defBHex}>";


        protected static int xSize = 200, ySize = 50;

        protected static string[] colors = new string[1000];
        protected static int colorPoint = 1;

        protected static int[,] frontColorMap = new int[ySize, xSize];
        protected static int[,] backColorMap = new int[ySize, xSize];
        protected static char[][] charMap = new char[ySize][];
        protected static bool[,] refreshMap = new bool[ySize, xSize + 1];

        static Renderer()
        {
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = defFHex;
            }
            colors[1] = defBHex;

            for (int i = 0; i < ySize; i++)
            {
                charMap[i] = new char[xSize + 1];
                charMap[i][xSize] = ' ';
                for (int j = 0; j < xSize; j++)
                {
                    charMap[i][j] = ' ';
                    frontColorMap[i, j] = 0;
                    backColorMap[i, j] = 1;
                    refreshMap[i, j] = false;
                }
            }
        }

        public static void WriteRaw(string str, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        }

        public static void DrawFrame()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < ySize; y++)
            {
                Console.WriteLine(new string(charMap[y]));
            }
        }

        public static void DrawRFrame()
        {

            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(refreshMap[y, x] ? 'O' : 'X');
                }
            }
        }
        public static void Refresh()
        {
            int startpos = -1;
            int endpos = -1;
            int strPos = 0;
            int curBCol;
            int curFCol;
            int prevBCol = -1;
            int prevFCol = -1;
            string retStr;
            for (int y = 0; y < ySize; y++)
            {
                //Console.WriteLine();
                // Console.Write($"{y}");
                if (refreshMap[y, xSize])
                {
                    //Console.Write($"!");
                    retStr = "";
                    for (int x = 0; startpos < 0 && x < xSize; x++)
                    {
                        if (refreshMap[y, x])
                        {
                            startpos = x;
                        }
                    }
                    if (startpos >= 0)
                    {
                        for (int x = xSize - 1; x >= 0 && endpos < 0 && x >= startpos; x--)
                        {
                            if (refreshMap[y, x])
                            {
                                endpos = x + 1;

                            }
                        }

                        prevFCol = frontColorMap[y, startpos];
                        prevBCol = backColorMap[y, startpos];
                        strPos = startpos;
                        for (int x = startpos; x <= endpos; x++)
                        {
                            curFCol = frontColorMap[y, x];
                            curBCol = backColorMap[y, x];
                            if (curFCol != prevFCol || prevBCol != curBCol || x == endpos)
                            {
                                WriteRaw(colors[prevFCol], 0, 42);
                                retStr += new string(charMap[y], strPos, x - strPos);//.Pastel(colors[prevFCol]).PastelBg(colors[prevBCol]);

                                strPos = x;
                                prevBCol = curBCol;
                                prevFCol = curFCol;
                            }
                        }

                        WriteRaw(retStr, startpos, y);
                    }
                }
            }
        }
        
        protected static int TryAddColor(string hex)
        {
            colorPoint++;
            if (colorPoint >= colors.Length)
                colorPoint = 2;

            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == hex)
                    return i;
            }
            colors[colorPoint] = hex;

            return colorPoint;
        }
        public static void Write(string str, int x, int y)
        {
            str = str.Replace("</cf>", defFHexTag);
            str = str.Replace("</cb>", defBHexTag);
            string curFHex = "FFFFFF";
            string curBHex = "000000";
            string retStr = "";
            string tStr;
            int pos = str.IndexOf("<c");
            int prevPos = 0;
            bool isBackground = false;
            int xOffset = 0;
            while (pos >= 0)
            {
                if (str[pos + 9] != '>')
                {
                    continue;
                }

                if (str[pos + 2] == 'f')
                {
                    isBackground = false;

                }
                else if (str[pos + 2] == 'b')
                {
                    isBackground = true;
                }
                else
                {
                    throw new InvalidFormatException();
                }

                if (pos >= prevPos && pos != prevPos)
                {

                    retStr += str.Substring(prevPos, pos - prevPos).Colors(curFHex, curBHex);
                    //Console.WriteLine(str.Substring(prevPos, str.Length - prevPos));
                    WriteOnCanvas(str.Substring(prevPos, pos - prevPos), curFHex, curBHex, x + xOffset, y);
                    xOffset += pos - prevPos;
                }

                if (isBackground)
                {
                    curBHex = str.Substring(pos + 3, 6);
                    TryAddColor(curBHex);
                }
                else
                {
                    curFHex = str.Substring(pos + 3, 6);
                    TryAddColor(curFHex);
                }

                prevPos = pos + 10;
                pos = str.IndexOf("<c", prevPos);
            }
            if (str.Length >= prevPos && pos != prevPos)
            {
                retStr += str.Substring(prevPos, str.Length - prevPos).Colors(curFHex, curBHex);
                //Console.WriteLine(str.Substring(prevPos, str.Length - prevPos));
                WriteOnCanvas(str.Substring(prevPos, str.Length - prevPos), curFHex, curBHex, x + xOffset, y);
            }
            //Console.WriteLine(retStr);
        }

        protected static void WriteOnCanvas(string text, string fHex, string bHex, int x, int y)
        {
            int tCol = 0;
            bool tRef;
            for (int i = 0; i < text.Length && x < xSize; i++)
            {
                tRef = false;
                if (charMap[y][x] != text[i])
                {
                    charMap[y][x] = text[i];
                    tRef = true;
                }
                tCol = TryAddColor(fHex);
                if (frontColorMap[y, x] != tCol)
                {
                    frontColorMap[y, x] = tCol;
                    tRef = true;
                }
                tCol = TryAddColor(bHex);
                if (backColorMap[y, x] != tCol)
                {
                    backColorMap[y, x] = tCol;
                    tRef = true;
                }
                refreshMap[y, x] = tRef;
                if (tRef)
                {
                    refreshMap[y, xSize] = true;
                }
                x++;
            }
        }


    }

    class InvalidFormatException : Exception
    {
        public InvalidFormatException()
        {
        }

        public InvalidFormatException(string message)
            : base(message)
        {
        }

        public InvalidFormatException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
