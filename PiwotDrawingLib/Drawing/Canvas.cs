using Pastel;
using PiwotToolsLib.PMath;
using System;

namespace PiwotDrawingLib.Drawing
{
    public class Canvas
    {
        string defFHex = "FFFFFF";
        string defBHex = "000000";
        string defFHexTag = $"<cfFFFFFF>";
        string defBHexTag = $"<cb000000>";


        int drawCOunt = 0;
        int[,] frameFrontColorMap;
        int[,] frameBackColorMap;
        char[][] frameCharMap;

        int[,] canvasFrontColorMap;
        int[,] canvasBackColorMap;
        char[][] canvasCharMap;

        bool[,] refreshMap;
        int colorPoint;

        string[] colorDict;

        bool needsRedraw;

        Int2 size = new Int2(0,0);


        public int ColorCunt
        {
            get
            {
                if(colorDict != null)
                    return colorDict.Length;
                return 0;
            }
            set
            {
                if(value < 4)
                {
                    throw new ArgumentOutOfRangeException();
                }
                string[] newDict = new string[value];
                if(colorDict != null)
                {
                    for (int i = 0; i < newDict.Length && i < colorDict.Length; i++)
                    {
                        newDict[i] = colorDict[i];
                    }
                    for (int i = colorDict.Length; i < newDict.Length; i++)
                    {
                        newDict[i] = defFHex;
                    }
                    colorDict = newDict;
                }
            }
        }

        public Canvas(Int2 size, int colorCount)
        {
            needsRedraw = true;

            colorDict = new string[colorCount];

            for (int i = 0; i < colorDict.Length; i++)
            {
                colorDict[i] = defFHex;
            }
            colorDict[1] = defBHex;

            Resize(size);
           
            colorPoint = 2;
        }

        public void Draw(string str, int x, int y)
        {
            str = str.Replace("</cf>", defFHexTag);
            str = str.Replace("</cb>", defBHexTag);
            string curFHex = "FFFFFF";
            string curBHex = "000000";
            int pos = str.IndexOf("<c");
            int prevPos = 0;
            bool isBackground;
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
                    //Rendering.Renderer.Write(str.Substring(prevPos, str.Length - prevPos) + " ", 60, 1);
                    Draw(str.Substring(prevPos, pos - prevPos), curFHex, curBHex, x + xOffset, y);
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
                //Rendering.Renderer.Write(str.Substring(prevPos, str.Length - prevPos) + " ", 60, 1);
                Draw(str.Substring(prevPos, str.Length - prevPos), curFHex, curBHex, x + xOffset, y);
            }
            //Console.WriteLine(retStr);
        }


        public void Draw(string text, string foregroundHex, string backgroundHex, int x, int y)
        {
            if (y >= size.Y)
            {
                return;
            }
            for (int i = 0; i < text.Length && x < size.X; i++)
            {

                frameCharMap[y][x] = text[i];

                frameFrontColorMap[y, x] = TryAddColor(foregroundHex);

                frameBackColorMap[y, x] = TryAddColor(backgroundHex);

                x++;

            }
            refreshMap[y, size.X] = true;

        }

        public void ApplyNewFrame()
        {
            for (int y = 0; y < size.Y; y++)
            {
                if (refreshMap[y, size.X])
                {
                    refreshMap[y, size.X] = false;
                    for (int x = 0; x < size.X; x++)
                    {
                        CheckOnePixel(x, y);
                    }
                }
            }
        }

        protected void CheckOnePixel(int x, int y)
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
                refreshMap[y, x]  = true;
                refreshMap[y, size.X] = true;
            }
        }

        public void Print()
        {
            int startpos;
            int endpos;
            int strPos;
            int curBCol;
            int curFCol;
            int prevBCol;
            int prevFCol;
            string retStr;
            ApplyNewFrame();
            drawCOunt++;
            //Rendering.Renderer.SyncWrite($"STOP 7: {canvasSize}  ", 100, 7);
            //DrawMap();
            for (int y = 0; y < size.Y; y++)
            {
                //Console.WriteLine();
                // Console.Write($"{y}");
                Renderer.SyncWrite(refreshMap[y, size.X] + " ", 0, y);
                if (refreshMap[y, size.X])
                {
                    //Console.Write($"!");
                    startpos = -1;
                    endpos = -1;
                    retStr = "";
                    //Rendering.Renderer.Write("STOP 9", 100, 9);
                    for (int x = 0; startpos < 0 && x < size.X; x++)
                    {
                        //Renderer.SyncWrite(" ", x + 50, y);
                        //Rendering.Renderer.Write($" {x} : {refreshMap[y, x]} ", 130, 10);
                        if (refreshMap[y, x])
                        {
                            startpos = x;
                        }
                    }
                    //Rendering.Renderer.Write($"STOP 10 {startpos} ", 100, 10);
                    if (startpos >= 0)
                    {
                        for (int x = size.X - 1; x >= 0 && endpos < 0 && x >= startpos; x--)
                        {
                            //Renderer.SyncWrite(" ", x + 50, y);
                            //Rendering.Renderer.Write($"STOP 11 {x}  ", 100, 11);
                            if (refreshMap[y, x])
                            {
                                endpos = x;
                            }
                        }

                        prevFCol = canvasFrontColorMap[y, startpos];
                        prevBCol = canvasBackColorMap[y, startpos];
                        strPos = startpos;
                        for (int x = startpos; x <= endpos; x++)
                        {
                            //Rendering.Renderer.Write($"STOP 12 {x}  ", 100, 12);
                            curFCol = canvasFrontColorMap[y, x];
                            curBCol = canvasBackColorMap[y, x];
                            if (curFCol != prevFCol || prevBCol != curBCol || x == endpos)
                            {
                                retStr += new string(canvasCharMap[y], strPos, x - strPos + 1).Pastel(colorDict[prevFCol]).PastelBg(colorDict[prevBCol]);

                                strPos = x;
                                prevBCol = curBCol;
                                prevFCol = curFCol;
                            }
                            //Renderer.SyncWrite("X", x + 50, y);
                        }

                        Renderer.SyncWrite(retStr, startpos, y);
                        for (int i = startpos; i <= endpos; i++)
                            refreshMap[y, i] = false;
                        refreshMap[y, size.X] = false;
                    }


                }
            }
            needsRedraw = false;
            Console.ReadKey(true);
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
        public void Resize(Int2 newSize)
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

            for (int i = 0; i < newSize.Y && i < size.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = 0; j < newSize.X && j < size.X; j++)
                {
                    newFrameCharMap[i][j] = frameCharMap[i][j];
                    newFrameFrontColorMap[i, j] = frameFrontColorMap[i,j];
                    newFrameBackColorMap[i, j] = frameBackColorMap[i,j];

                    newCanvasCharMap[i][j] = canvasCharMap[i][j];
                    newCanvasFrontColorMap[i, j] = canvasFrontColorMap[i, j];
                    newCanvasBackColorMap[i, j] = canvasBackColorMap[i,j];

                    newRefreshMap[i, j] = refreshMap[i,j];
                }
            }

            for (int i = size.Y; i < newSize.Y; i++)
            {
                newFrameCharMap[i] = new char[newSize.X + 1];
                newFrameCharMap[i][newSize.X] = ' ';

                newCanvasCharMap[i] = new char[newSize.X + 1];
                newCanvasCharMap[i][newSize.X] = ' ';

                for (int j = size.X; j < newSize.X; j++)
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
            size = new Int2(newSize);
            frameFrontColorMap = newFrameFrontColorMap;
            frameBackColorMap = newFrameBackColorMap;
            frameCharMap = newFrameCharMap;
            
            canvasFrontColorMap = newCanvasFrontColorMap;
            canvasBackColorMap = newCanvasBackColorMap;
            canvasCharMap = newCanvasCharMap;

            refreshMap = newRefreshMap;
        }

    }
}
