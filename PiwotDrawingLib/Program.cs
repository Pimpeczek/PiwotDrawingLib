using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            Renderer.WindowSize = new Int2(200, 50);
            Renderer.DebugMode = true;
            Renderer.AsyncMode = true;
            Renderer.AsyncFrameLenght = 30;


            UI.Containers.Canvas c = new UI.Containers.Canvas(new Int2(2, 2), new Int2(40, 40), "Test canvas", Misc.Boxes.BoxType.dashed);
            c.Draw();
            Console.ReadKey(true);
            char ch;
            for (int i = 0; i < 1000; i++)
            {
                
                for (int j = 0; j < 150; j++)
                {
                    ch = (char)(65 + (i + j / 100) % 26);
                    c.Draw(""+ch, j % 38, (j / 38) % 38);
                    //c.DrawMap();
                    c.RefreshContent();
                    Console.ReadKey(true);
                }
                c.RefreshContent();
            }
            
            /*
            Int2 menuSize = new Int2(40, 20);
            UI.Containers.Menu mainMenu = new UI.Containers.Menu(new Int2((Renderer.WindowSize.X - menuSize.X) / 2, (Renderer.WindowSize.Y - menuSize.X) / 2), menuSize, "Main menu", Misc.Boxes.BoxType.round);
            //mainMenu.VerticalTextWrapping = UI.Containers.Menu.Wrapping.scrolling;

            UI.Controls.ButtonControl bc = new UI.Controls.ButtonControl("b0", "b0");
            bc.AddAction("b1", (x) => { Renderer.AddDissapearingText($"XDDD {DateTime.Now.Millisecond}", 1000, new Int2()); return true; });
            mainMenu.AddControl(bc);

            mainMenu.AddControl(new UI.Controls.LineSeparatorControl("L0", "_label_0"));

            bc = new UI.Controls.ButtonControl("b1", "b1");
            bc.AddAction("b1", (x) => { Renderer.AddDissapearingText($"DXXX {DateTime.Now.Millisecond}", 1000, new Int2(0, 1)); return true; });
            mainMenu.AddControl(bc);

            UI.Controls.CheckBoxControl cb = new UI.Controls.CheckBoxControl("CB", "_CB", true)
            {
                TrueValue = "XD",
                FalseValue = "DX",
                HideName = true
            };
            mainMenu.AddControl(cb);

            UI.Controls.IntSwitcherControl isc = new UI.Controls.IntSwitcherControl("ICS", "_ICS", 0, 0, 10000000, 1)
            {
                MinSpecialText = "ZERO",
                MaxSpecialText = "DUŻO",
                FastStepMultiplier = 10,
                FastStepsToMultiply = 10,
                FastStepTime = 1000,
                HideName = true
            };
            mainMenu.AddControl(isc);

            UI.Controls.FloatSwitcherControl fsc = new UI.Controls.FloatSwitcherControl("FCS", "_FCS", 0, 0, 10000000, 0.5f)
            {
                MinSpecialText = "ZERO",
                MaxSpecialText = "DUŻO",
                FastStepMultiplier = 10,
                FastStepsToMultiply = 10,
                FastStepTime = 200,
                HideName = true,
                RoundingDigits = 5
            };
            mainMenu.AddControl(fsc);

            mainMenu.WaitForInput();
            */
            //Renderer.AbortAsyncThread();

            Console.ReadKey(true);

        }
    }
}
