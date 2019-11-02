﻿using Pastel;
using PiwotToolsLib.PMath;
using System;

namespace PiwotDrawingLib.UI.Containers
{
    class Canvas : Container
    {
        protected string defFHex = "FFFFFF";
        protected string defBHex = "000000";
        protected string defFHexTag = $"<cfFFFFFF>";
        protected string defBHexTag = $"<cb000000>";



        protected int[,] frameFrontColorMap;
        protected int[,] frameBackColorMap;
        protected char[][] frameCharMap;

        protected int[,] canvasFrontColorMap;
        protected int[,] canvasBackColorMap;
        protected char[][] canvasCharMap;

        protected bool[,] refreshMap;
        protected int colorPoint;

        protected string[] colorDict;

        protected bool needsRedraw;
        protected bool ready = false;
        protected bool canvasNeedsRedraw;




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
            canvasNeedsRedraw = true;
            //Rendering.Renderer.Write($"{canvasPosition}", 100,1);
           
            //Rendering.Renderer.Write($"{canvasPosition}", 100, 2);
            needsRedraw = true;

            frameFrontColorMap = new int[contentSize.Y, contentSize.X];
            frameBackColorMap = new int[contentSize.Y, contentSize.X];
            frameCharMap = new char[contentSize.Y][];

            canvasFrontColorMap = new int[contentSize.Y, contentSize.X];
            canvasBackColorMap = new int[contentSize.Y, contentSize.X];
            canvasCharMap = new char[contentSize.Y][];

            refreshMap = new bool[contentSize.Y, contentSize.X + 1];
            colorDict = new string[256];
            for (int i = 0; i < colorDict.Length; i++)
            {
                colorDict[i] = defFHex;
            }
            colorDict[1] = defBHex;

            for (int i = 0; i < contentSize.Y; i++)
            {
                frameCharMap[i] = new char[contentSize.X + 1];
                frameCharMap[i][contentSize.X] = ' ';

                canvasCharMap[i] = new char[contentSize.X + 1];
                canvasCharMap[i][contentSize.X] = ' ';

                for (int j = 0; j < contentSize.X; j++)
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
            colorPoint = 2;
            ready = true;
        }

        public void Draw(string str, int x, int y)
        {
            str = str.Replace("</cf>", defFHexTag);
            str = str.Replace("</cb>", defBHexTag);
            string curFHex = "FFFFFF";
            string curBHex = "000000";
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
                    //Rendering.Renderer.Write(str.Substring(prevPos, str.Length - prevPos) + " ", 60, 1);
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
                //Rendering.Renderer.Write(str.Substring(prevPos, str.Length - prevPos) + " ", 60, 1);
                WriteOnCanvas(str.Substring(prevPos, str.Length - prevPos), curFHex, curBHex, x + xOffset, y);
            }
            //Console.WriteLine(retStr);
        }

        protected void WriteOnCanvas(string text, string fHex, string bHex, int x, int y)
        {
            if (y >= contentSize.Y)
            { 
                return;
            }
            for (int i = 0; i < text.Length && x < contentSize.X; i++)
            {
                
                frameCharMap[y][x] = text[i];

                frameFrontColorMap[y, x] = TryAddColor(fHex);

                frameBackColorMap[y, x] = TryAddColor(bHex);
                
                x++;
                
            }
            refreshMap[y, contentSize.X] = true;

        }

        protected void ApplyNewFrame()
        {
            for (int y = 0; y < contentSize.Y; y++)
            {
                if (refreshMap[y, contentSize.X]) {
                    refreshMap[y, contentSize.X] = false;
                    for (int x = 0; x < contentSize.X; x++)
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

            if(tFlag)
            {
                refreshMap[y, x] = refreshMap[y, contentSize.X] = true;

            }
        }

        public virtual void RefreshContent()
        {
            DrawContent();
        }

        public void DrawMap()
        {
            Drawing.Renderer.Draw(DateTime.Now.Millisecond, 60, 1);
            for (int i = 0; i < contentSize.Y; i++)
            {
                Drawing.Renderer.Draw(new string(frameCharMap[i]), 60, 2 + i);

                for (int j = 0; j <= contentSize.X; j++)
                {
                    Drawing.Renderer.Draw(refreshMap[i, j] ? "X" : " ", 90 + j, 2 + i);
                }
            }

        }

        protected override void DrawContent()
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

            //Rendering.Renderer.SyncWrite($"STOP 7: {canvasSize}  ", 100, 7);
            //DrawMap();
            for (int y = 0; y < contentSize.Y; y++)
            {
                //Console.WriteLine();
                // Console.Write($"{y}");
                //Rendering.Renderer.Write($"STOP 8: {y}  ", 100, 8);
                if (refreshMap[y, contentSize.X])
                {
                    //Console.Write($"!");
                    startpos = -1;
                    endpos = -1;
                    retStr = "";
                    //Rendering.Renderer.Write("STOP 9", 100, 9);
                    for (int x = 0; startpos < 0 && x < contentSize.X; x++)
                    {
                        //Rendering.Renderer.Write($" {x} : {refreshMap[y, x]} ", 130, 10);
                        if (refreshMap[y, x])
                        {
                            startpos = x;
                        }
                    }
                    //Rendering.Renderer.Write($"STOP 10 {startpos} ", 100, 10);
                    if (startpos >= 0)
                    {
                        for (int x = contentSize.X - 1; x >= 0 && endpos < 0 && x >= startpos; x--)
                        {
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
                        }

                        Drawing.Renderer.Draw(retStr, startpos + contentPosition.X, y + contentPosition.Y);
                        for (int i = startpos; i <= endpos; i++)
                            refreshMap[y, i] = false;
                        refreshMap[y, contentSize.X] = false;
                    }


                }
            }
            canvasNeedsRedraw = false;
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Drawing.Renderer.Draw(Name, position.X + (size.X - Name.Length) / 2, position.Y);
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
