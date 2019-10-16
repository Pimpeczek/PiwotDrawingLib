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
            Int2 menuSize = new Int2(40, 5);
            UI.Containers.Menu mainMenu = new UI.Containers.Menu(new Int2((Renderer.WindowSize.X - menuSize.X) / 2, (Renderer.WindowSize.Y - menuSize.X) / 2), menuSize, "Main menu", Misc.Boxes.BoxType.doubled);
            //mainMenu.VerticalTextWrapping = UI.Containers.Menu.Wrapping.scrolling;
            mainMenu.AddControl(new UI.Controls.LineSeparatorControl("sep0", "sep0"));
            mainMenu.AddControl(new UI.Controls.MenuControl("Host a game", "host"));
            //mainMenu.GetControll("host").AddAction(HostAGame);
            mainMenu.AddControl(new UI.Controls.MenuControl("Ships", "ships"));
            mainMenu.AddControl(new UI.Controls.MenuControl("Ship designer", "designer"));
            mainMenu.AddControl(new UI.Controls.LineSeparatorControl("sep1", "sep1"));
            mainMenu.AddControl(new UI.Controls.MenuControl("Exit", "exit"));

            mainMenu.GetControll("exit").AddAction(mainMenu.Exit);

            mainMenu.AddControl(new UI.Controls.LineSeparatorControl("sep_fin", "sep_fin"));
            mainMenu.WaitForInput();

            //Renderer.AbortAsyncThread();

            Console.ReadKey(true);

        }
    }
}
