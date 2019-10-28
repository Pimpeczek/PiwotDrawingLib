using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Pastel;
using System.IO;
using System.Diagnostics;
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

            /*
            UI.Containers.FunctionDisplay fd = new UI.Containers.FunctionDisplay(new Int2(0, 0), new Int2(200, 50), "Main menu", Misc.Boxes.BoxType.round, (x) => x);
            fd.Draw();
            float f;
            long time = 0;
            Stopwatch stopwatch = new Stopwatch();
            for(int i = 0; i < 100; i++)
            {
                stopwatch.Restart();
                for (int j = 0; j < 1000; j++)
                {
                    f = (float)Rand.Double();
                    fd.Function = (x) => (x + f - 0.5f) * (x + f - 0.5f);
                    
                    fd.RefreshContent();
                }
                stopwatch.Stop();
                fd.TestDraw();
                
                Renderer.Write(stopwatch.ElapsedMilliseconds + " ",0,0);
                time += stopwatch.ElapsedMilliseconds;
            }
            time /= 100;
            Renderer.Write(time + " ", 0, 0);
            */
            /*
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 1000; i++)
            {
                ch = (char)(65 + i % 26);
                sw.Restart();
                for (int j = 0; j < 10000; j++)
                {
                    c.Draw(""+ch, j % 38, (j / 38) % 38);
                    //c.DrawMap();
                    //Console.ReadKey(true);
                }
                c.RefreshContent();
                sw.Stop();
                Renderer.Write(sw.ElapsedMilliseconds + " ", 40, 0);
                Console.ReadKey(true);
            }
            */
            
            
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

            UI.Controls.IntSwitcherControl isc = new UI.Controls.IntSwitcherControl("ICS", "_ICS", 40, 2, 10000000, 1)
            {
                MinSpecialText = "ZERO",
                MaxSpecialText = "DUŻO",
                FastStepMultiplier = 10,
                FastStepsToMultiply = 10,
                FastStepTime = 1000,
                HideName = true,

            };
            isc.AddAction("ISC", (x) => { mainMenu.Size = new Int2(((UI.Events.IntSwitcherEvent)x).Value, 20); return true; });
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
            
            //Renderer.AbortAsyncThread();
            
            Console.ReadKey(true);

        }
    }
}
