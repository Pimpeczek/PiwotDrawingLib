using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using PiwotToolsLib.PMath;
using PiwotToolsLib.PGraphics;

namespace PiwotDrawingLib.UI.Containers
{
    public class PictureBox : Container
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
                DrawContent();
            }
        }

        public PictureBox() : base(new Int2(), new Int2(10, 10), "Menu", Misc.Boxes.BoxType.doubled)
        {
            Setup(new Bitmap(8, 16));
        }

        public PictureBox(Int2 position, Int2 size, string name, Misc.Boxes.BoxType boxType, Bitmap image) : base(position, size, name, boxType)
        {
            Setup(image);
        }

        void Setup(Bitmap image)
        {
            Image = image;
            sizeDifferenceHandling = ContentHandling.ResizeContent;
        }

        /// <summary>
        /// Orders the Renderer to print visual representation of the function.
        /// </summary>
        public void RefreshContent()
        {
            DrawContent();
        }

        protected override void DrawContent()
        {
            int yPos;
            int yDrawPos;
            if (bitmap.Height % 2 == 1)
            {
                yDrawPos = bitmap.Height / 2;
                yPos = bitmap.Height - 1;
                for (int x = bitmap.Width - 1; x >= 0; x--)
                {
                    Drawing.Renderer.Draw("▄", "000000", GetHex(bitmap.GetPixel(x, yPos)), contentPosition.X + x, contentPosition.Y + yDrawPos);
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
                    Drawing.Renderer.Draw("▄", GetHex(bitmap.GetPixel(x, y + 1)), GetHex(bitmap.GetPixel(x, y)), contentPosition.X + x, contentPosition.Y + y / 2);
                }
            }
        }

        protected void CutColorBits()
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb((int)((bitmap.GetPixel(x, y).ToArgb()) & usedMod)));
                }
            }
        }

        protected string GetHex(Color c)
        {
            return c.ToArgb().ToString("X");
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Drawing.Renderer.Draw(Name, "FFFFFF", "000000", position.X + (size.X - Name.Length) / 2, position.Y);
        }
        protected void RefreshBitmap()
        {
            if (image == null)
            {
                bitmap = new Bitmap(contentSize.X, contentSize.Y * 2);
            }

            if (sizeDifferenceHandling == ContentHandling.CropContent)
            {
                bitmap = Bitmaper.CropBitmap(image, 0, 0, contentSize.X, contentSize.Y * 2);
            }
            else if (sizeDifferenceHandling == ContentHandling.FitContent)
            {
                bitmap = Bitmaper.ResizeToFit(image, contentSize.X, contentSize.Y * 2, Color.Black);
            }
            else
            {
                bitmap = Bitmaper.StreachToSize(image, contentSize.X, contentSize.Y * 2);
            }
            CutColorBits();
        }
    }
}
