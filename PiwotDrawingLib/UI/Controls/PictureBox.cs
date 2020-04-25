using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using PiwotToolsLib.PMath;
using static PiwotDrawingLib.UI.Containers.Container;
using PiwotDrawingLib.Drawing;

namespace PiwotDrawingLib.UI.Controls
{
    public class PictureBox: UIElement
    {
        static uint usedMod = 0xFFFFFFFF;
        static readonly uint mod24 = 0xFFFFFFFF;
        static readonly uint mod21 = 0xFFFEFEFE;
        static readonly uint mod18 = 0xFFFCFCFC;
        static readonly uint mod15 = 0xFFF8F8F8;
        static readonly uint mod12 = 0xFFF0F0F0;
        static readonly uint mod9 = 0xFFE0E0E0;
        static readonly uint mod6 = 0xFFC0C0C0;
        static readonly uint mod3 = 0xFF808080;
        static readonly uint[] mods = new uint[8] { mod24, mod21, mod18, mod15, mod12, mod9, mod6, mod3 };


        public enum ColorEncoding { Bit24, Bit21, Bit18, Bit15, Bit12, Bit9, Bit6, Bit3 };

        private ColorEncoding bitsPerColor;

        /// <summary>
        /// How many bits should be used to store information about color on single pixel. Each color(R, G, B) gests a third of given bit count.
        /// </summary>
        public ColorEncoding BitsPerColor
        {
            get
            {
                return bitsPerColor;
            }
            set
            {
                if (value == bitsPerColor)
                    return;
                bitsPerColor = value;
                usedMod = mods[(int)bitsPerColor];
                RefreshBitmap();
            }
        }

        protected Bitmap bitmap;

        protected string[,] colorMap;

        protected Bitmap image;

        /// <summary>
        /// The image used as a base for this PictureBox.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                RefreshBitmap();
            }
        }

        protected ContentHandling sizeDifferenceHandling;

        /// <summary>
        /// Determines how and if the PictureBox should resize the given source image.
        /// </summary>
        public ContentHandling SizeDifferenceHandling
        {
            get
            {
                return sizeDifferenceHandling;
            }
            set
            {
                if (value == sizeDifferenceHandling)
                    return;
                sizeDifferenceHandling = value;
                RefreshBitmap();
            }
        }

        public PictureBox() : base()
        {
            Setup(new Int2(), new Int2(10, 10), new Bitmap(8, 16));
        }

        public PictureBox(Int2 position, Int2 size, Bitmap image) : base()
        {
            Setup(position, size, image);
        }

        void Setup(Int2 position, Int2 size, Bitmap image)
        {
            Position = position;
            Size = size;
            Image = image;
            contentRedrawNeeded = true;
            sizeDifferenceHandling = ContentHandling.ResizeContent;
        }

        protected void CutColorBits()
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    colorMap[x, y] = GetHex(Color.FromArgb((int)((bitmap.GetPixel(x, y).ToArgb()) & usedMod)));
                }
            }
        }

        protected string GetHex(Color c)
        {
            return c.ToArgb().ToString("X");
        }

        protected void RefreshBitmap()
        {
            if (image == null)
            {
                bitmap = new Bitmap(size.X, size.Y * 2);
                colorMap = new string[size.X, size.Y * 2];
            }

            if (sizeDifferenceHandling == ContentHandling.CropContent)
            {
                bitmap = PiwotToolsLib.PGraphics.Bitmaper.CropBitmap(image, 0, 0, size.X, size.Y * 2);
            }
            else if (sizeDifferenceHandling == ContentHandling.FitContent)
            {
                bitmap = PiwotToolsLib.PGraphics.Bitmaper.ResizeToFit(image, size.X, size.Y * 2, Color.Black);
            }
            else
            {
                bitmap = PiwotToolsLib.PGraphics.Bitmaper.StreachToSize(image, size.X, size.Y * 2);
            }
            colorMap = new string[bitmap.Width, bitmap.Height];
            CutColorBits();
            contentRedrawNeeded = true;
        }

        public override void PrintOnCanvas(Canvas canvas)
        {
            if (!contentRedrawNeeded)
                return;
            int yPos;
            int yDrawPos;
            if (bitmap.Height % 2 == 1)
            {
                yDrawPos = bitmap.Height / 2;
                yPos = bitmap.Height - 1;
                for (int x = bitmap.Width - 1; x >= 0; x--)
                {
                    canvas.DrawOnCanvas("▄", "000000", colorMap[x, yPos], position.X + x, position.Y + yDrawPos);
                }
            }
            else
            {
                yPos = bitmap.Height - 2;
            }

            for (int x = bitmap.Width - 1; x >= 0; x--)
            {
                for (int y = yPos; y >= 0; y -= 2)
                {
                    canvas.DrawOnCanvas("▄", colorMap[x, y + 1], colorMap[x, y], position.X + x, position.Y + y / 2);
                }
            }
        }
    }

}
