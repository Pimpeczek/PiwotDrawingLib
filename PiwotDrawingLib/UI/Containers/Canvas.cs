using System;
using System.Drawing;
using Pastel;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Containers
{
    class Canvas: Container
    {
        protected string defFHex = "FFFFFF";
        protected string defBHex = "000000";
        protected string defFHexTag = $"<cfFFFFFF>";
        protected string defBHexTag = $"<cb000000>";



        protected int[,] frontColorMap;
        protected int[,] backColorMap;
        protected char[][] charMap;
        protected bool[,] refreshMap;
        protected int colorPoint;

        protected string[] colorDict;

        protected bool needsRedraw;

        protected Int2 canvasSize;
        protected Int2 canvasPosition;


        public Canvas() : base(new Int2(), new Int2(10, 10), "Canvas", Misc.Boxes.BoxType.doubled)
        {
            IsVIsable = false;
            Setup();
            IsVIsable = true;
        }

        public Canvas(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType) : base(position, size, name, boxType)
        {
            Setup();
        }

        void Setup()
        {
            canvasSize = new Int2(Size);
            canvasPosition = new Int2(Position);
            if (boxType != Misc.Boxes.BoxType.none)
            {
                canvasSize -= Int2.One * 2;
            }
            else
            {
                canvasPosition += Int2.One;
            }
            needsRedraw = true;
            frontColorMap = new int[canvasSize.Y, canvasSize.X];
            backColorMap = new int[canvasSize.Y, canvasSize.X];
            charMap = new char[canvasSize.Y][];
            refreshMap = new bool[canvasSize.Y, canvasSize.X + 1];
            colorDict = new string[256];
            for (int i = 0; i < colorDict.Length; i++)
            {
                colorDict[i] = defFHex;
            }
            colorDict[1] = defBHex;

            for (int i = 0; i < canvasSize.Y; i++)
            {
                charMap[i] = new char[canvasSize.X + 1];
                charMap[i][canvasSize.X] = ' ';
                for (int j = 0; j < canvasSize.X; j++)
                {
                    charMap[i][j] = ' ';
                    frontColorMap[i, j] = 0;
                    backColorMap[i, j] = 1;
                    refreshMap[i, j] = false;
                }
            }
            colorPoint = 2;
        }

        public void Draw(string str, int x, int y)
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
                    throw new Exceptions.InvalidFormatException();
                }

                if (pos >= prevPos && pos != prevPos)
                {

                    //retStr += str.Substring(prevPos, pos - prevPos).Pastel(curFHex).PastelBg(curBHex);
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
                //retStr += str.Substring(prevPos, str.Length - prevPos).Pastel(curFHex).PastelBg(curBHex);
                //Console.WriteLine(str.Substring(prevPos, str.Length - prevPos));
                WriteOnCanvas(str.Substring(prevPos, str.Length - prevPos), curFHex, curBHex, x + xOffset, y);
                Rendering.Renderer.Write(str.Substring(prevPos, str.Length - prevPos), 0, 1);
            }
            //Console.WriteLine(retStr);
        }

        protected void WriteOnCanvas(string text, string fHex, string bHex, int x, int y)
        {
            int tCol = 0;
            bool tRef;
            for (int i = 0; i < text.Length && x < canvasSize.X; i++)
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
                    refreshMap[y, canvasSize.X] = true;
                }
                x++;
            }
        }

        public void RefreshContent()
        {
            DrawContent();
        }

        protected override void DrawContent()
        {
            int startpos = -1;
            int endpos = -1;
            int strPos = 0;
            int curBCol;
            int curFCol;
            int prevBCol = -1;
            int prevFCol = -1;
            string retStr;
            for (int y = 0; y < canvasSize.Y; y++)
            {
                //Console.WriteLine();
                // Console.Write($"{y}");
                if (refreshMap[y, canvasSize.X])
                {
                    //Console.Write($"!");
                    retStr = "";
                    for (int x = 0; startpos < 0 && x < canvasSize.X; x++)
                    {
                        if (refreshMap[y, x])
                        {
                            startpos = x;
                        }
                    }
                    if (startpos >= 0)
                    {
                        for (int x = canvasSize.X - 1; x >= 0 && endpos < 0 && x >= startpos; x--)
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
                                retStr += new string(charMap[y], strPos, x - strPos).Pastel(colorDict[prevFCol]).PastelBg(colorDict[prevBCol]);

                                strPos = x;
                                prevBCol = curBCol;
                                prevFCol = curFCol;
                            }
                        }

                        Rendering.Renderer.Write(retStr, startpos + Position.X, y + Position.Y);
                    }
                    for(int i = startpos; i <= endpos; i++)
                        refreshMap[y, i] = false;
                    refreshMap[y, canvasSize.X] = false;
                }
            }
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);
        }
        
        protected int TryAddColor(string hex)
        {
            colorPoint++;
            if (colorPoint >= colorDict.Length)
                colorPoint = 2;

            for (int i = 0; i < colorDict.Length; i++)
            {
                if (colorDict[i] == hex)
                    return i;
            }
            colorDict[colorPoint] = hex;

            return colorPoint;
        }
        
    }
}
