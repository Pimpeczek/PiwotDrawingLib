using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiwotToolsLib.PMath;
namespace PiwotDrawingLib.Drawing
{
    public class Region
    {
        public Int2 position;
        public Int2 Position
        {
            get
            {
                return new Int2(position);
            }
            set
            {
                position = new Int2(value ?? throw new ArgumentNullException());
            }
        }

        public Int2 size;
        public Int2 Size
        {
            get
            {
                return new Int2(size);
            }
            set
            {
                size = new Int2(value ?? throw new ArgumentNullException());
            }
        }
        public int X
        {
            get
            {
                return position.X;
            }
        }

        public int Y
        {
            get
            {
                return position.Y;
            }
        }

        public int Width
        {
            get
            {
                return size.X;
            }
        }

        public int Height
        {
            get
            {
                return size.Y;
            }
        }

        public Region()
        {
            position = new Int2();
            size = new Int2();
        }

        public Region(Int2 position, Int2 size)
        {
            this.Position = position;
            this.Size = size;
        }

        public Region(int x, int y, int width, int height)
        {
            Position = new Int2(x, y);
            Size = new Int2(width, height);
        }
    }
}
