using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiwotDrawingLib.Rendering;

namespace PiwotDrawingLib
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            
            Renderer.WindowSize = new PiwotLib.Math.Int2(200, 60);
            Renderer.DebugMode = true;
            Renderer.AsyncMode = true;
            Renderer.AsyncFrameLenght = 10;
            int counter = 0;
            Renderer.Write($"{counter}".PadLeft(10), 0, 0);
            Console.ReadKey(true);
            while(true)
            {
                for(int i = 0; i < 100; i++)
                    Renderer.Write($"{counter++}".PadLeft(10), 0, 0);
                //Console.ReadKey(true);
            }
        }
    }
}
