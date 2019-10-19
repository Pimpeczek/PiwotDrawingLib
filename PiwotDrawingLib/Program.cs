using System;
using System.Collections.Generic;
using Pastel;
using System.IO;
using System.Drawing;
using System.Threading;
using PiwotDrawingLib.Rendering;
using PiwotToolsLib.PMath;

namespace PiwotDrawingLib
{
    class Program
    {
        static void Main(string[] args)
        {
            Renderer.WindowSize = new Int2(200, 60);
            Renderer.DebugMode = true;
            //Renderer.AsyncMode = true;
            Renderer.AsyncFrameLenght = 30;
            Int2 menuSize = new Int2(40, 20);
            UI.Containers.Menu mainMenu = new UI.Containers.Menu(new Int2((Renderer.WindowSize.X - menuSize.X) / 2, (Renderer.WindowSize.Y - menuSize.X) / 2), menuSize, "Main menu", Misc.Boxes.BoxType.doubled);
            //mainMenu.VerticalTextWrapping = UI.Containers.Menu.Wrapping.scrolling;

            UI.Controls.ButtonControl bc = new UI.Controls.ButtonControl("b0", "b0");
            bc.AddAction("b1", (x) => { Renderer.AddDissapearingText($"XDDD {DateTime.Now.Millisecond}", 1000, new Int2()); return true; });
            mainMenu.AddControl(bc);

            mainMenu.AddControl(new UI.Controls.LineSeparatorControl("L0", "_label_0"));

            bc = new UI.Controls.ButtonControl("b1", "b1");
            bc.AddAction("b1", (x) => { Renderer.AddDissapearingText($"DXXX {DateTime.Now.Millisecond}", 1000, new Int2(0,1)); return true; });
            mainMenu.AddControl(bc);

            UI.Controls.CheckBoxControl cb = new UI.Controls.CheckBoxControl("CB", "_CB", true)
            {
                TrueValue = "XD",
                FalseValue = "DX",
                HideName = true
            };
            mainMenu.AddControl(cb);

            mainMenu.WaitForInput();

            //Renderer.AbortAsyncThread();

            Console.ReadKey(true);

        }
    }
}
