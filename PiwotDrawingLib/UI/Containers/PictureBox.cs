using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using PiwotToolsLib.PMath;
using PiwotToolsLib.PGraphics;

namespace PiwotDrawingLib.UI.Containers
{
    class PictureBox : Container
    {
        protected Bitmap bitmap;
        protected Bitmap image;
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

        protected ContentHandling overflowHandling;
        public ContentHandling OverflowHandling
        {
            get
            {
                return overflowHandling;
            }
            set
            {
                if (value == overflowHandling)
                    return;
                overflowHandling = value;
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
            overflowHandling = ContentHandling.ResizeContent;
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
            if(bitmap.Height % 2 == 1 )
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
                for (int y = yPos; y >= 0 ; y-=2)
                {
                    Drawing.Renderer.Draw("▄", GetHex(bitmap.GetPixel(x, y+1)), GetHex(bitmap.GetPixel(x, y)), contentPosition.X + x, contentPosition.Y + y /2);
                } 
            }
            
        }
        /*
         11111111 FF 255
         1000 0000 80 128
         1100 0000 C0 192
         1110 0000 E0 224
         1111 0000 F0 240
         1111 1000 F8 248
         1111 1100 FC 252
         1111 1110 FE 254
         1111 1111 FF 255
          
         
        */




        static uint mod0 = 0xFFFFFFFF;
        static uint mod24 = 0xFFFFFFFF;
        static uint mod21 = 0xFFFEFEFE;
        static uint mod18 = 0xFFFCFCFC;
        static uint mod15 = 0xFFF8F8F8;
        static uint mod12 = 0xFFF0F0F0;
        static uint mod9 = 0xFFE0E0E0;
        static uint mod6 = 0xFFC0C0C0;
        static uint mod3 = 0xFF808080;
        

        protected string GetHex(Color c)
        {
            return (c.ToArgb() & mod18).ToString("X");
            return c.ToArgb().ToString("X").Substring(2);
            //return $"{c.R.ToString("X").PadLeft(2, '0')}{c.G.ToString("X").PadLeft(2, '0')}{c.B.ToString("X").PadLeft(2, '0')}";
        }

        protected override void DrawWindow()
        {
            base.DrawWindow();
            Drawing.Renderer.Draw(Name, "FFFFFF", "000000", position.X + (size.X - Name.Length) / 2, position.Y);
        }
        protected void RefreshBitmap()
        {
            if(image == null)
            {
                bitmap = new Bitmap(contentSize.X, contentSize.Y * 2);
            }

            if(overflowHandling == ContentHandling.CropContent)
            {
                bitmap = Bitmaper.CropBitmap(image, 0,0, contentSize.X, contentSize.Y * 2);
            }
            else
            {
                bitmap = Bitmaper.ResizeBitmap(image, contentSize.X, contentSize.Y * 2);

            }
        }
    }
}
