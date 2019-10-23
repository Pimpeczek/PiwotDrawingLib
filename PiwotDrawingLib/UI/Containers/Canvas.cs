using System;
using System.Drawing;
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

        protected string[] colorDict;

        protected bool needsRedraw;

        protected Int2 canvasSize;


        public Canvas() : base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            IsVIsable = false;
            Setup();
            IsVIsable = true;
        }

        public Canvas(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType, Func<float, float> func) : base(position, size, name, boxType)
        {
            Setup();
        }

        void Setup()
        {
            canvasSize = new Int2(Size);
            if(boxType != Misc.Boxes.BoxType.none)
            {
                canvasSize.X -= 2;
                canvasSize.Y -= 2;
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
        }

        public void Draw()
        {

        }

        protected override void DrawContent()
        {
            throw new NotImplementedException();
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Rendering.Renderer.Write(Name, Position.X + (Size.X - Name.Length) / 2, Position.Y);
        }
    }
}
