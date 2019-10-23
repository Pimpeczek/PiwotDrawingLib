using System;
using System 
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib.UI.Containers
{
    class Canvas: Container
    {
        protected char[][] characters;
        protected int[,] colors;
        protected Colo

        public Canvas() : base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            IsVIsable = false;
            Setup();
            IsVIsable = true;
        }

        public Canvas(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType, Func<float, float> func) : base(position, size, name, boxType)
        {
            Setup();
            xD.
        }

        void Setup()
        {


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
