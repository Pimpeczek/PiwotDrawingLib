using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Pastel;
using System.IO;
using System.Drawing;
using System.Threading;
using PiwotDrawingLib.Drawing;
using PiwotToolsLib.PMath;
using System.Text;
using System.Activities;
using System.Runtime.InteropServices;
using PiwotDrawingLib.UI.Containers;

namespace PiwotDrawingLib
{
    class Program
    {
        //[DllImport("kernel32.dll", SetLastError = true)]
        //private static extern IntPtr GetConsoleWindow();
        static void Main(string[] args)
        {

            Renderer.WindowSize = new Int2(200, 50);
            Renderer.FrameLenght = 30;
            double t = 1;
            UI.Containers.ScrollContainer sc = new UI.Containers.ScrollContainer(Int2.Zero, new Int2(100, 50), new Int2(98, 48), "SVXD", Misc.Boxes.BoxType.round);
            UI.Controls.SimpleFunctionDisplay pictureBox = new UI.Controls.SimpleFunctionDisplay(Int2.Zero, new Int2(98, 48), (x) => 0.01f / x);


            //sc.AddChild(pictureBox);
            sc.Register();
            //Console.ReadKey(true);
            bool pingpong = true;
            while (true)
            {
                

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        //sc.ScrollUp();
                        break;
                    case ConsoleKey.DownArrow:
                        //sc.ScrollDown();
                        break;
                    case ConsoleKey.LeftArrow:
                        //sc.ScrollLeft();
                        //t++;
                        sc.Size = sc.Size + Int2.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        //sc.ScrollRight();
                        //t--;
                        sc.Size = sc.Size + Int2.Right;
                        break;
                    case ConsoleKey.P:
                        pingpong = !pingpong;
                        break;
                     
                }
                pictureBox.Function = (x) => (float)Math.Sin(x + t/100);
                sc.Draw();
            }
        }
    }
}
